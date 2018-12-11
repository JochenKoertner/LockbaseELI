using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ui.Models;

namespace ui.Controllers
{
	[Route("api/[controller]")]
	public class DataController : Controller
	{
		[HttpGet("[action]")]
		public IEnumerable<PersonInfo> Persons()
		{
			yield return new PersonInfo 
			{
				Id = 4711, 
				Name = "Ahrens; Andrea",
				Section = "Geschäftsleitung",
				Summary = "keine zeitliche Einschränkung"
			};

			yield return new PersonInfo 
			{
				Id = 4712,
				Name = "Müller; Bernd",
				Section = "Werkstattsleitung",
				Summary = "keine zeitliche Einschränkung"
			};

			yield return new PersonInfo 
			{
				Id = 4713, 
				Name = "Schmidt; Helga",
				Section = "Sekretariat",
				Summary = "normale Öffnungszeiten"
			};

		}

		[HttpGet("[action]")]
		public IEnumerable<DoorInfo> Doors()
		{
			yield return new DoorInfo 
			{
				Id = 4711, 
				Name = "Büro Barthauer",
				Image = "buero_barthauer"
			};

		}
	}
}
