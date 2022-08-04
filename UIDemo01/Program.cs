// See https://aka.ms/new-console-template for more information

using WA.Holidays.SDK;

var container = new HolidayContainer();
var data =await container.未來的非六日假日();

foreach (HolidayContainer.HolidayVM holidayVM in data)
{
	Console.WriteLine($"距離 {holidayVM.Name} 還剩 {holidayVM.DiffDays} 天, Date={holidayVM.Date:yyyy/MM/dd}");
}

Console.ReadKey();