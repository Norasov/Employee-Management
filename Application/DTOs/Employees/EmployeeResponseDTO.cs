namespace Application.DTOs.Employees;

public class EmployeeResponseDTO
{
    public int Id {get;set;}
    public string FirstName {get;set;} = null!;
    public string LastName {get;set;} = null!;
    public string Email {get;set;} = null!;
    public DateTime HireDate {get;set;}
    public decimal Salary {get;set;}
    public bool IsActive {get;set;}
    public string DepartmentName {get;set;}=null!;
    public string PositionTitle {get;set;} = null!;
}
