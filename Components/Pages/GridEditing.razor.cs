using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudFarmework.Models;

namespace MudFarmework.Components.Pages
{
    public partial class GridEditing : ComponentBase
    {
        private List<EmployeeModel> employees = new();
        private List<string> departments = new() { "IT", "HR", "Finance", "Marketing", "Sales" };
        private List<string> availableSkills = new() { "C#", "JavaScript", "Python", "SQL", "React", "Angular", "Vue", "Docker", "Kubernetes", "AWS" };
        private List<string> changeLogs = new(); // For debugging

        protected override void OnInitialized()
        {
            // Khởi tạo dữ liệu mẫu
            employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    Id = 1,
                    Name = "Nguyễn Văn A",
                    Age = 30,
                    Salary = 50000000,
                    HireDate = DateTime.Now.AddYears(-2),
                    Department = "IT",
                    Skills = new List<string> { "C#", "SQL", "JavaScript" }
                },
                new EmployeeModel
                {
                    Id = 2,
                    Name = "Trần Thị B",
                    Age = 28,
                    Salary = 45000000,
                    HireDate = DateTime.Now.AddYears(-1),
                    Department = "HR",
                    Skills = new List<string> { "Python", "SQL" }
                },
                new EmployeeModel
                {
                    Id = 3,
                    Name = "Lê Văn C",
                    Age = 35,
                    Salary = 60000000,
                    HireDate = DateTime.Now.AddYears(-3),
                    Department = "Finance",
                    Skills = new List<string> { "C#", "React", "Docker" }
                }
            };
        }

        private void AddNewRow()
        {
            var newEmployee = new EmployeeModel
            {
                Id = employees.Count > 0 ? employees.Max(x => x.Id) + 1 : 1,
                Name = "",
                Age = 25,
                Salary = 30000000,
                HireDate = DateTime.Now,
                Department = departments.First(),
                Skills = new List<string>()
            };

            employees.Add(newEmployee);
            StateHasChanged();
        }

        private void DeleteRow(EmployeeModel employee)
        {
            employees.Remove(employee);
            StateHasChanged();
        }

        // ✅ MAIN METHOD - OnCellChange với improved error handling
        private async Task OnCellChange(EmployeeModel item, object newValue, string propertyName)
        {
            try
            {
                var logMessage = $"{DateTime.Now:HH:mm:ss} - OnCellChange: ID={item.Id}, {propertyName} = {newValue ?? "NULL"}";
                changeLogs.Add(logMessage);
                Console.WriteLine($"🔧 {logMessage}");

                // Validate trước khi update
                if (!ValidateChange(item, newValue, propertyName))
                {
                    var errorMsg = $"{DateTime.Now:HH:mm:ss} - ❌ Validation failed for {propertyName}";
                    changeLogs.Add(errorMsg);
                    Console.WriteLine($"❌ Validation failed for {propertyName}");
                    return;
                }

                // Update property value
                switch (propertyName.ToLower())
                {
                    case "name":
                        item.Name = newValue?.ToString() ?? "";
                        break;

                    case "age":
                        if (int.TryParse(newValue?.ToString(), out int age))
                            item.Age = age;
                        break;

                    case "salary":
                        if (decimal.TryParse(newValue?.ToString(), out decimal salary))
                            item.Salary = salary;
                        break;

                    case "hiredate":
                        var dateValue = ConvertToDateTime(newValue);
                        if (dateValue.HasValue)
                        {
                            item.HireDate = dateValue.Value;
                        }
                        break;

                    case "department":
                        item.Department = newValue?.ToString() ?? "";
                        break;

                    case "skills":
                        if (newValue is IEnumerable<string> skills)
                        {
                            item.Skills = skills.ToList();
                        }
                        else if (newValue is List<string> skillsList)
                        {
                            item.Skills = skillsList;
                        }
                        else
                        {
                            item.Skills = new List<string>();
                        }
                        break;

                    default:
                        var unknownMsg = $"{DateTime.Now:HH:mm:ss} - ❌ Unknown property: {propertyName}";
                        changeLogs.Add(unknownMsg);
                        Console.WriteLine($"❌ Unknown property: {propertyName}");
                        return;
                }

                // Tìm và update item trong list
                var index = employees.FindIndex(e => e.Id == item.Id);
                if (index >= 0)
                {
                    employees[index] = item;
                    var successMsg = $"{DateTime.Now:HH:mm:ss} - ✅ Updated employee at index {index}: {item.Name}";
                    changeLogs.Add(successMsg);
                    Console.WriteLine($"✅ Updated employee at index {index}: {item.Name}");

                    // Trigger CommittedItemChanges
                    ItemHasBeenCommitted(item);

                    // Refresh UI
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    var notFoundMsg = $"{DateTime.Now:HH:mm:ss} - ❌ Employee not found in list: ID={item.Id}";
                    changeLogs.Add(notFoundMsg);
                    Console.WriteLine($"❌ Employee not found in list: ID={item.Id}");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"{DateTime.Now:HH:mm:ss} - ❌ Exception in OnCellChange: {ex.Message}";
                changeLogs.Add(errorMsg);
                Console.WriteLine($"❌ Exception in OnCellChange: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // ✅ OVERLOAD METHODS - với null safety
        private async Task OnCellChange(EmployeeModel item, string newValue, string propertyName)
        {
            await OnCellChange(item, (object)newValue, propertyName);
        }

        private async Task OnCellChange(EmployeeModel item, int newValue, string propertyName)
        {
            await OnCellChange(item, (object)newValue, propertyName);
        }

        private async Task OnCellChange(EmployeeModel item, decimal newValue, string propertyName)
        {
            await OnCellChange(item, (object)newValue, propertyName);
        }

        private async Task OnCellChange(EmployeeModel item, DateTime? newValue, string propertyName)
        {
            await OnCellChange(item, (object)newValue, propertyName);
        }

        private async Task OnCellChange(EmployeeModel item, IEnumerable<string> newValue, string propertyName)
        {
            await OnCellChange(item, (object)newValue, propertyName);
        }

        // NEW: Overload cho List<string>
        private async Task OnCellChange(EmployeeModel item, List<string> newValue, string propertyName)
        {
            await OnCellChange(item, (object)newValue, propertyName);
        }

        // ✅ IMPROVED VALIDATION với null safety
        private bool ValidateChange(EmployeeModel item, object newValue, string propertyName)
        {
            try
            {
                switch (propertyName.ToLower())
                {
                    case "name":
                        return !string.IsNullOrWhiteSpace(newValue?.ToString());

                    case "age":
                        if (int.TryParse(newValue?.ToString(), out int age))
                            return age >= 18 && age <= 100;
                        return false;

                    case "salary":
                        if (decimal.TryParse(newValue?.ToString(), out decimal salary))
                            return salary >= 0;
                        return false;

                    case "hiredate":
                        if (newValue is DateTime || newValue is DateTime?)
                            return true;
                        return false;

                    case "department":
                        var deptValue = newValue?.ToString();
                        return !string.IsNullOrEmpty(deptValue) && departments.Contains(deptValue);

                    case "skills":
                        if (newValue is IEnumerable<string> skills)
                            return skills.All(s => availableSkills.Contains(s));
                        if (newValue is List<string> skillsList)
                            return skillsList.All(s => availableSkills.Contains(s));
                        return false;

                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Validation error for {propertyName}: {ex.Message}");
                return false;
            }
        }

        // ✅ ORIGINAL EVENTS
        private void ItemHasBeenCommitted(EmployeeModel item)
        {
            var message = $"✅ CommittedItemChanges: {item.Name} - {item.Department}";
            changeLogs.Add($"{DateTime.Now:HH:mm:ss} - {message}");
            Console.WriteLine(message);

            // Có thể thêm logic save vào database ở đây
            // await SaveToDatabase(item);
        }

        private void OnStartedEditingItem(EmployeeModel item)
        {
            var message = $"🔧 StartedEditingItem: {item.Name}";
            changeLogs.Add($"{DateTime.Now:HH:mm:ss} - {message}");
            Console.WriteLine(message);
        }

        private void OnCancelledEditingItem(EmployeeModel item)
        {
            var message = $"❌ CancelledEditingItem: {item.Name}";
            changeLogs.Add($"{DateTime.Now:HH:mm:ss} - {message}");
            Console.WriteLine(message);
        }

        // ✅ HELPER METHOD để convert DateTime
        private DateTime? ConvertToDateTime(object value)
        {
            try
            {
                if (value == null) return null;

                // Direct DateTime
                if (value is DateTime dateTime)
                    return dateTime;

                // DateTime? (nullable)
                if (value.GetType() == typeof(DateTime?))
                {
                    var nullableDate = (DateTime?)value;
                    return nullableDate;
                }

                // String
                if (value is string dateString && DateTime.TryParse(dateString, out DateTime parsedDate))
                    return parsedDate;

                // Try convert from object
                if (DateTime.TryParse(value.ToString(), out DateTime convertedDate))
                    return convertedDate;

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error converting to DateTime: {ex.Message}");
                return null;
            }
        }
    }
}