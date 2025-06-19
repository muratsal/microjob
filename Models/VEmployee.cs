using System;
using System.Collections.Generic;

#nullable disable

namespace NanoGo.Models
{
    public partial class VEmployee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime WorkStartDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int TitleId { get; set; }
        public string Title { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string GsmNumber { get; set; }
        public string Image { get; set; }
        public bool IsWorking { get; set; }
        public DateTime? EndWorkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public string IntegrationCode { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? ResponsibleFor { get; set; }
        public string ResponsibleForName { get; set; }
    }
}
