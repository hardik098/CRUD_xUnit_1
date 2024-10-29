using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service_contracts;
using Service_contracts.DTO;
using Service_contracts.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace CRUD_xUnit_1.Controllers
{
    // [Route("persons")]  // Prefix persons should be added in url for each route
    [Route("[controller]")]  // Prefix persons should be added in url for each route
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsController(ICountriesService countriesService, IPersonsService personsService)
        {
            _countriesService = countriesService;
            _personsService = personsService;
        }

        //[Route("persons/index")]
        [Route("index")]
        //[Route("/index")] when '/' is added before index, it will overwrite 'persons' prefix, and url will be '/index' only to access the view
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string? sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)    // Parameter name must be same as input attributes's name in the view, which will get through query string
        {
            // To display itmes in dropdown list
            ViewBag.SearchFields = new Dictionary<string, string>()
             {
                { nameof(PersonResponse.PersonName), "Person Name" },   // property name, value to be display in UI
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
             };

            //List<PersonResponse> pesrons = _personsService.GetAllPersons();
            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString); // Method returns all persons if no parameter is given

            ViewBag.CurrentSearchBy = searchBy; // To display search by in dropdown list after performing serach, because after search it is setting to first item in the list
            ViewBag.CurrentSearchString = searchString; // To display search string in search text box after performing serach, because after search search-text will be set to empty


            // Sort
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons);
        }


        //  Executes when the user clicks on "Create Person" hyperlink (while opening the create view)

        [Route("[action]")] // It will take action name automatically
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCoutries();
            //ViewBag.Countries = countries;

            // or
            ViewBag.Countries = countries.Select(
                temp => new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryId.ToString()
                }

                );

            return View();
        }
        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCoutries();
                ViewBag.Countries = countries;

                // Get errors, and display below submit button in view

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }
            PersonResponse response = _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

    }
}
