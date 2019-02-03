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
			yield return new PersonInfo { 
				value = "900-1", label = "Ahrens, Andrea", department = "Geschäftsführung", color = "OliveDrab" };
			yield return new PersonInfo {
				value = "901-1", label = "Barthauer, Thomas", department = "Geschäftsführung", color = "SandyBrown" };
			yield return new PersonInfo {
				value = "103-1", label = "Fendler, Klaus", department = "Buchhaltung" };
			yield return new PersonInfo {
				value = "104-1", label = "Kistler, Sabine", department = "Vertrieb" };
			yield return new PersonInfo {
				value = "105-1", label = "Kohl, Ulrich", department = "Vertrieb" };
			yield return new PersonInfo {
				value = "200-1", label = "Leinkamp, Sebastian", department = "Lager" };
			yield return new PersonInfo {
				value = "201-1", label = "Mertens, Martina", department = "Lager" };
			yield return new PersonInfo {
				value = "202-1", label = "Sidow, Janin", department = "Montage" };
			yield return new PersonInfo {
				value = "203-1", label = "Walter, Jens", department = "Montage" };
			yield return new PersonInfo {
				value = "203-2", label = "Winter, Sina", department = "Montage" };
			yield return new PersonInfo {
				value = "203-3", label = "Wondraschek, Volker", department = "Montage" };
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
