
using Newtonsoft.Json;

namespace WA.Holidays.SDK
{
	public class Facade
	{
		public async Task<IEnumerable<HolidayFromJson>> GetData()
		{
			string url =
				"https://quality.data.gov.tw/dq_download_json.php?nid=145708&md5_url=1a7506b007086d03afe37570ac7f52e4";

			using var response = await new HttpClient().GetAsync(url);
			response.EnsureSuccessStatusCode();

			var contentString = await response.Content.ReadAsStringAsync();

			var holidays = JsonConvert.DeserializeObject<List<HolidayFromJson>>(contentString);

			return holidays;
		}
	}

	public class HolidayFromJson
	{
		public DateTime Date { get; set; }
		public string Name { get; set; }
		public string IsHoliday { get; set; }
		public string HolidayCategory { get; set; }
		public string Description { get; set; }
	}

	public class JsonHolidaysAdapter
	{
		public async Task<IEnumerable<Holiday>> GetHolidays()
		{
			var facade = new Facade();
			var jsonData = await facade.GetData();

			return jsonData.Select(x => x.ToHoliday());
		}
	}
	public class Holiday
	{
		public DateTime Date { get; set; }
		public string Name { get; set; }
		public bool IsHoliday { get; set; }
		public string HolidayCategory { get; set; }
		public bool IsSaturdayOrSunday => HolidayCategory == "星期六、星期日";
		public override string ToString()
		{
			return $"{Date.ToString("yyyy/MM/dd")}, Name={Name}, IsHoliday={IsHoliday}, 是六日={IsSaturdayOrSunday}, category={HolidayCategory}";
		}
	}

	public static class HolidayFromJsonExts
	{
		public static Holiday ToHoliday(this HolidayFromJson source)
		{
			return new Holiday
			{
				Date = source.Date,
				Name = source.Name,
				IsHoliday = source.IsHoliday == "是" ? true : false,
				HolidayCategory = source.HolidayCategory,
			};
		}
	}

	public class HolidayContainer
	{
		public async Task<IEnumerable<HolidayVM>> 未來的非六日假日()
		{
			var allHolidays= await new JsonHolidaysAdapter().GetHolidays();
			return allHolidays
				.Where(h => h.IsSaturdayOrSunday == false && h.Date >= DateTime.Today)
				.Select(h => new HolidayVM { Date = h.Date, Name = h.Name});
		}

		public class HolidayVM
		{
			public DateTime Date { get; set; }
			public string Name { get; set; }
			public int DiffDays => (Date - DateTime.Today).Days;
		}
	}
}