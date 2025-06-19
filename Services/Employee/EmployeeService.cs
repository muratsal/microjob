using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class EmployeeService
    {

        public List<ItemForCombo> GetListForCombo(string search, int? value,bool allData)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.Employees.
                       Where(x => x.FullName.Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.FullName,
                           Value = x.EmployeeId,
                           FreeText = x.EmployeeId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.Employees.
                      Where(x => x.EmployeeId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.FullName,
                          Value = x.EmployeeId,
                          FreeText = x.EmployeeId.ToString()

                      }).ToList();
                }
                else if (allData)
                {
                    itemComboList = dbContext.Employees.
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.FullName,
                           Value = x.EmployeeId,
                           FreeText = x.EmployeeId.ToString()

                       }).ToList();
                }


                return itemComboList;
            }
        }

        public PageList<EmployeeDTO> GetEmployeeList(EmployeeFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.VEmployees.OrderByDescending(x => x.EmployeeId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.EmployeeId.HasValue) dbQuery = dbQuery.Where(x => x.EmployeeId == filter.EmployeeId.Value);
                    if (!string.IsNullOrEmpty(filter.FirstName)) dbQuery = dbQuery.Where(x => x.FirstName.Contains(filter.FirstName));
                    if (!string.IsNullOrEmpty(filter.LastName)) dbQuery = dbQuery.Where(x => x.LastName.Contains(filter.LastName));
                    if (!string.IsNullOrEmpty(filter.FullName)) dbQuery = dbQuery.Where(x => x.FullName.Contains(filter.FullName));
                    if (filter.WorkStartDate.HasValue) dbQuery = dbQuery.Where(x => x.WorkStartDate >= filter.WorkStartDate.Value);
                    if (filter.WorkStartDate2.HasValue) dbQuery = dbQuery.Where(x => x.WorkStartDate <= filter.WorkStartDate2.Value);
                    if (filter.BirthDate.HasValue) dbQuery = dbQuery.Where(x => x.BirthDate >= filter.BirthDate.Value);
                    if (filter.BirthDate2.HasValue) dbQuery = dbQuery.Where(x => x.BirthDate <= filter.BirthDate2.Value);
                    if (filter.TitleId.HasValue) dbQuery = dbQuery.Where(x => x.TitleId == filter.TitleId.Value);
                    if (filter.DepartmentId.HasValue) dbQuery = dbQuery.Where(x => x.DepartmentId == filter.DepartmentId.Value);
                    if (filter.UserId.HasValue) dbQuery = dbQuery.Where(x => x.UserId == filter.UserId.Value);
                    if (!string.IsNullOrEmpty(filter.EmailAddress)) dbQuery = dbQuery.Where(x => x.EmailAddress.Contains(filter.EmailAddress));
                    if (!string.IsNullOrEmpty(filter.PhoneNumber)) dbQuery = dbQuery.Where(x => x.PhoneNumber.Contains(filter.PhoneNumber));
                    if (!string.IsNullOrEmpty(filter.GsmNumber)) dbQuery = dbQuery.Where(x => x.GsmNumber.Contains(filter.GsmNumber));
                    if (!string.IsNullOrEmpty(filter.Image)) dbQuery = dbQuery.Where(x => x.Image.Contains(filter.Image));
                    if (filter.IsWorking.HasValue) dbQuery = dbQuery.Where(x => x.IsWorking == filter.IsWorking.Value);
                    if (filter.EndWorkDate.HasValue) dbQuery = dbQuery.Where(x => x.EndWorkDate >= filter.EndWorkDate.Value);
                    if (filter.EndWorkDate2.HasValue) dbQuery = dbQuery.Where(x => x.EndWorkDate <= filter.EndWorkDate2.Value);
                    if (filter.CreatedDate.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate >= filter.CreatedDate.Value);
                    if (filter.CreatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate <= filter.CreatedDate2.Value);
                    if (filter.CreatedUser.HasValue) dbQuery = dbQuery.Where(x => x.CreatedUser == filter.CreatedUser.Value);
                    if (filter.UpdatedDate.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate >= filter.UpdatedDate.Value);
                    if (filter.UpdatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate <= filter.UpdatedDate2.Value);
                    if (filter.UpdatedUser.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedUser == filter.UpdatedUser.Value);
                }

                totalCount = dbQuery.Count();

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.OrderBy))
                    {
                        string clientOrderByName = queryInfo.OrderBy.StartsWith("-") ? queryInfo.OrderBy.Substring(1) : queryInfo.OrderBy;
                        string orderByName = typeof(Models.Employee).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

                        if (queryInfo.OrderBy.StartsWith("-"))
                        {
                            dbQuery = dbQuery.OrderByDescending(p => EF.Property<object>(p, orderByName));
                        }
                        else
                        {
                            dbQuery = dbQuery.OrderBy(p => EF.Property<object>(p, orderByName));
                        }
                    }
                }

                if (!isExport)
                {
                    if (queryInfo != null && queryInfo.Pager != null)
                    {
                        dbQuery = dbQuery.Skip((queryInfo.Pager.CurrentPage) * queryInfo.Pager.PageSize).Take(queryInfo.Pager.PageSize);
                    }
                }

                var data = dbQuery.ToList()
                     .Select(x => new EmployeeDTO
                     {
                         EmployeeId = x.EmployeeId,
                         FirstName = x.FirstName,
                         LastName = x.LastName,
                         FullName = x.FullName,
                         WorkStartDate = x.WorkStartDate,
                         BirthDate = x.BirthDate,
                         TitleId = x.TitleId,
                         Title = x.Title,
                         DepartmentId = x.DepartmentId,
                         Department = x.Department,
                         UserId = x.UserId,
                         UserName = x.UserName,
                         EmailAddress = x.EmailAddress,
                         PhoneNumber = x.PhoneNumber,
                         GsmNumber = x.GsmNumber,
                         Image = x.Image,
                         IsWorking = x.IsWorking,
                         EndWorkDate = x.EndWorkDate,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName,
                         IntegrationCode = x.IntegrationCode,
                         CompanyId =x.CompanyId,
                         CompanyName =x.CompanyName,
                         ResponsibleFor =x.ResponsibleFor,
                         ResponsibleForName=x.ResponsibleForName,
                         
                        
                     }).ToList();

                return new PageList<EmployeeDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public EmployeeDTO SaveEmployee(EmployeeDTO employeeDTO, UserInfo userInfo)
        {
            Models.Employee employee = new Models.Employee();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = employeeDTO.EmployeeId == 0;
                if (isNewRecord)
                {
                    employee.CreatedDate = DateTime.Now;
                    employee.CreatedUser = userInfo.UserId;
                }
                else
                {
                    employee = dbContext.Employees.First(x => x.EmployeeId == employeeDTO.EmployeeId);

                }

                employee.UpdatedDate = DateTime.Now;
                employee.UpdatedUser = userInfo.UserId;


                employee.FirstName = employeeDTO.FirstName;
                employee.LastName = employeeDTO.LastName;
                employee.FullName = employeeDTO.FirstName +" "+ employeeDTO.LastName;
                employee.WorkStartDate = employeeDTO.WorkStartDate;
                employee.BirthDate = employeeDTO.BirthDate;
                employee.TitleId = employeeDTO.TitleId;
                employee.DepartmentId = employeeDTO.DepartmentId;
                employee.CompanyId = employeeDTO.CompanyId;
                employee.UserId = ( employeeDTO.UserId.HasValue && employeeDTO.UserId.Value == 0 ) ? null : employeeDTO.UserId;
                employee.EmailAddress = employeeDTO.EmailAddress;
                employee.PhoneNumber = employeeDTO.PhoneNumber;
                employee.GsmNumber = employeeDTO.GsmNumber;
                employee.Image = employeeDTO.Image;
                employee.IsWorking = employeeDTO.IsWorking;
                employee.EndWorkDate = employeeDTO.EndWorkDate;
                employee.IntegrationCode = employeeDTO.IntegrationCode;
                employee.ResponsibleFor = employeeDTO.ResponsibleFor;

                if (isNewRecord)
                {
                    dbContext.Employees.Add(employee);
                }

                dbContext.SaveChanges();

                employeeDTO.EmployeeId = employee.EmployeeId;
            }

            return employeeDTO;
        }


        public void DeleteEmployee(EmployeeDTO employeeDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var employee = dbContext.Employees.FirstOrDefault(x => x.EmployeeId == employeeDTO.EmployeeId);
                if (employee != null)
                {
                    dbContext.Employees.Remove(employee);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class EmployeeDTO
    {
        public Int32 EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get; set; }
        public DateTime WorkStartDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public Int32 TitleId { get; set; }
        public String Title { get; set; }
        public Int32 DepartmentId { get; set; }
        public String Department { get; set; }
        public Int32 CompanyId { get; set; }
        public String CompanyName { get; set; }
        public Int32? UserId { get; set; }
        public String UserName { get; set; }
        public String EmailAddress { get; set; }
        public String PhoneNumber { get; set; }
        public String GsmNumber { get; set; }
        public String Image { get; set; }
        public Boolean IsWorking { get; set; }
        public DateTime? EndWorkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
        public String IntegrationCode { get; set; }
        public Int32? ResponsibleFor { get; set; }
        public String ResponsibleForName { get; set; }
    }

    public class EmployeeFilter
    {
        public Int32? EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get; set; }
        public DateTime? WorkStartDate { get; set; }
        public DateTime? WorkStartDate2 { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? BirthDate2 { get; set; }
        public Int32? TitleId { get; set; }
        public Int32? DepartmentId { get; set; }
        public Int32? OfficeId { get; set; }
        public Int32? UserId { get; set; }
        public String EmailAddress { get; set; }
        public String PhoneNumber { get; set; }
        public String GsmNumber { get; set; }
        public String Image { get; set; }
        public Boolean? IsWorking { get; set; }
        public DateTime? EndWorkDate { get; set; }
        public DateTime? EndWorkDate2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
