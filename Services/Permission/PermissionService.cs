using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace NanoGo.Services.System
{
    public class PermissionService
    {
        public static IWebHostEnvironment hostEnvironment;
        public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.Permissions.
                       Where(x => x.PermissionId.ToString().Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.PermissionId.ToString(),
                           Value = x.PermissionId,
                           FreeText = x.PermissionId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.Permissions.
                      Where(x => x.PermissionId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.PermissionId.ToString(),
                          Value = x.PermissionId,
                          FreeText = x.PermissionId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<PermissionDTO> GetPermissionList(PermissionFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport, UserInfo _userInfo)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Permissions
                    .Include(x => x.Employee)
                    .Include(x => x.ApprovedWaitingEmployeeNavigation)
                    .Include(x => x.PermissionTypeNavigation)
                    .OrderByDescending(x => x.PermissionId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.PermissionId.HasValue) dbQuery = dbQuery.Where(x => x.PermissionId == filter.PermissionId.Value);
                    if (filter.EmployeeId.HasValue) dbQuery = dbQuery.Where(x => x.EmployeeId == filter.EmployeeId.Value);
                    if (filter.PermissionType.HasValue) dbQuery = dbQuery.Where(x => x.PermissionType == filter.PermissionType.Value);
                    if (filter.StartDate.HasValue) dbQuery = dbQuery.Where(x => x.StartDate >= filter.StartDate.Value);
                    if (filter.StartDate2.HasValue) dbQuery = dbQuery.Where(x => x.StartDate <= filter.StartDate2.Value);
                    if (filter.DayCount.HasValue) dbQuery = dbQuery.Where(x => x.DayCount == filter.DayCount.Value);
                    if (filter.EndDate.HasValue) dbQuery = dbQuery.Where(x => x.EndDate >= filter.EndDate.Value);
                    if (filter.EndDate2.HasValue) dbQuery = dbQuery.Where(x => x.EndDate <= filter.EndDate2.Value);
                    if (filter.Status.HasValue) dbQuery = dbQuery.Where(x => x.Status == filter.Status.Value);
                    if (!string.IsNullOrEmpty(filter.Description)) dbQuery = dbQuery.Where(x => x.Description.Contains(filter.Description));
                    if (filter.ProxyEmployee.HasValue) dbQuery = dbQuery.Where(x => x.ProxyEmployee == filter.ProxyEmployee.Value);
                    if (!string.IsNullOrEmpty(filter.Duties)) dbQuery = dbQuery.Where(x => x.Duties.Contains(filter.Duties));
                    if (!string.IsNullOrEmpty(filter.Note)) dbQuery = dbQuery.Where(x => x.Note.Contains(filter.Note));
                    if (filter.ApprovedWaitingEmployee.HasValue) dbQuery = dbQuery.Where(x => x.ApprovedWaitingEmployee == filter.ApprovedWaitingEmployee.Value);
                    if (filter.CreatedDate.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate >= filter.CreatedDate.Value);
                    if (filter.CreatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate <= filter.CreatedDate2.Value);
                    if (filter.CreatedUser.HasValue) dbQuery = dbQuery.Where(x => x.CreatedUser == filter.CreatedUser.Value);
                    if (filter.UpdatedDate.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate >= filter.UpdatedDate.Value);
                    if (filter.UpdatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate <= filter.UpdatedDate2.Value);
                    if (filter.UpdatedUser.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedUser == filter.UpdatedUser.Value);

                    if (filter.PageType.HasValue)
                    {
                        if (filter.PageType.Value == 1 && !_userInfo.AuthInfos.Any(x => x.AuthCode == "AUTH_ALLPERMISSION"))
                        {
                            dbQuery = dbQuery.Where(x => 1 == 0);
                        }
                        else if (filter.PageType.Value == 2)
                        {
                            if (_userInfo.EmployeeId.HasValue)
                                dbQuery = dbQuery.Where(x => x.EmployeeId == _userInfo.EmployeeId);
                            else
                                dbQuery = dbQuery.Where(x => 1 == 0);
                        }
                        else if (filter.PageType.Value == 3)
                        {
                            if (_userInfo.EmployeeId.HasValue)
                                dbQuery = dbQuery.Where(x => x.ApprovedWaitingEmployee == _userInfo.EmployeeId && x.Status == 2);
                            else
                                dbQuery = dbQuery.Where(x => 1 == 0);

                        }
                    }
                }

                totalCount = dbQuery.Count();

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.OrderBy))
                    {
                        string clientOrderByName = queryInfo.OrderBy.StartsWith("-") ? queryInfo.OrderBy.Substring(1) : queryInfo.OrderBy;
                        string orderByName = typeof(Models.Permission).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new PermissionDTO
                     {
                         PermissionId = x.PermissionId,
                         EmployeeId = x.EmployeeId,
                         EmployeeName = x.Employee.FullName,
                         PermissionType = x.PermissionType,
                         PermissionTypeText = x.PermissionTypeNavigation.ParamCode,
                         StartDate = x.StartDate,
                         DayCount = x.DayCount,
                         EndDate = x.EndDate,
                         Status = x.Status,
                         Description = x.Description,
                         ProxyEmployee = x.ProxyEmployee,
                         Duties = x.Duties,
                         Note = x.Note,
                         ApprovedWaitingEmployee = x.ApprovedWaitingEmployee,
                         ApprovedWaitingEmployeeName = x.ApprovedWaitingEmployeeNavigation != null ? x.ApprovedWaitingEmployeeNavigation.FullName : "",
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName

                     }).ToList();

                return new PageList<PermissionDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public PermissionDTO SavePermission(PermissionDTO permissionDTO, UserInfo userInfo)
        {
            Models.Permission permission = new Models.Permission();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = permissionDTO.PermissionId == 0;
                if (isNewRecord)
                {
                    permission.CreatedDate = DateTime.Now;
                    permission.CreatedUser = userInfo.UserId;
                }
                else
                {
                    permission = dbContext.Permissions.First(x => x.PermissionId == permissionDTO.PermissionId);

                }

                permission.UpdatedDate = DateTime.Now;
                permission.UpdatedUser = userInfo.UserId;


                permission.EmployeeId = permissionDTO.EmployeeId;
                permission.PermissionType = permissionDTO.PermissionType;
                permission.StartDate = permissionDTO.StartDate;
                permission.DayCount = permissionDTO.DayCount;
                permission.EndDate = permissionDTO.EndDate;
                permission.Status = permissionDTO.Status;
                permission.Description = permissionDTO.Description;
                permission.ProxyEmployee = permissionDTO.ProxyEmployee;
                permission.Duties = permissionDTO.Duties;
                permission.Note = permissionDTO.Note;
                permission.ApprovedWaitingEmployee = permissionDTO.ApprovedWaitingEmployee;





                if (isNewRecord)
                {
                    dbContext.Permissions.Add(permission);
                }

                dbContext.SaveChanges();

                permissionDTO.PermissionId = permission.PermissionId;
            }

            return permissionDTO;
        }

        public void ControlComfirmation(int permissionId, bool? comfirmed, UserInfo userInfo)
        {
            Models.Permission permission = new Models.Permission();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                permission = dbContext.Permissions.First(x => x.PermissionId == permissionId);
                if (permission.ApprovedWaitingEmployee.HasValue)
                {
                    var employee = dbContext.Employees.FirstOrDefault(x => x.EmployeeId == permission.ApprovedWaitingEmployee);
                    if (comfirmed.HasValue)
                    {
                        Models.PermissionLog pLog = new Models.PermissionLog();
                        pLog.PermissionId = permissionId;
                        pLog.CreatedUser = userInfo.UserId;
                        pLog.CreatedDate = DateTime.Now;
                        pLog.UpdatedDate = DateTime.Now;
                        pLog.UpdatedUser = userInfo.UserId;
                        pLog.Status = comfirmed.Value == true ? 1 : 0;
                        pLog.EmployeeId = userInfo.EmployeeId.Value;

                        dbContext.PermissionLogs.Add(pLog);

                        if (comfirmed.Value)
                        {
                            SendComfirmedEmail(permissionId, employee.EmployeeId, 1);
                            if (employee.ResponsibleFor.HasValue)
                            {
                                permission.ApprovedWaitingEmployee = employee.ResponsibleFor.Value;
                                permission.Status = 2;
                                SendComfirmEmail(permissionId, employee.ResponsibleFor.Value);
                            }
                            else
                            {
                                permission.Status = 3;
                                permission.ApprovedWaitingEmployee = null;
                            }
                        }
                        if (!comfirmed.Value)
                        {
                            permission.ApprovedWaitingEmployee = null;
                            permission.Status = 4;
                            SendComfirmedEmail(permissionId, employee.EmployeeId, 0);
                        }
                    }
                }
                else
                {
                    var employee = dbContext.Employees.FirstOrDefault(x => x.EmployeeId == permission.EmployeeId);

                    if (employee.ResponsibleFor.HasValue)
                    {
                        SendComfirmEmail(permissionId, employee.ResponsibleFor.Value);
                        permission.ApprovedWaitingEmployee = employee.ResponsibleFor.Value;
                        permission.Status = 2;
                    }
                    else
                    {
                        permission.Status = 3;
                    }
                }

                dbContext.SaveChanges();
            }

        }


        private void SendComfirmEmail(int permissionId, int responsibleEmployeeId)
        {
            try
            {
                string emailTemplate = GetComfirmEmailTemplate(permissionId);
                Models.Email email = new Models.Email();
                using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
                {
                    email.UpdatedDate = DateTime.Now;
                    email.UpdatedUser = 1;
                    email.CreatedDate = DateTime.Now;
                    email.CreatedUser = 1;

                    var reponsibleEmployee = dbContext.Employees.First(x => x.EmployeeId == responsibleEmployeeId);
                    var permission = dbContext.Permissions.Include(x => x.Employee).First(x => x.PermissionId == permissionId);

                    email.EmailFrom = dbContext.Parameters.Where(x => x.ParamType == 5 && x.ParamCode == "EMAIL_FROM").First().ParamDesc;
                    email.EmailTo = reponsibleEmployee.EmailAddress;
                    email.EmailToCc = permission.Employee.EmailAddress;
                    email.Subject = dbContext.Parameters.Where(x => x.ParamType == 5 && x.ParamCode == "EMAIL_SUBJECT_COMFIRM").First().ParamDesc;

                    email.Body = emailTemplate;
                    email.IsHtml = true;
                    email.IsSend = false;
                    email.IsSuccess = null;

                    dbContext.Emails.Add(email);
                    dbContext.SaveChanges();
                }

            }
            catch
            {

            }
        }

        private void SendComfirmedEmail(int permissionId, int responsibleEmployeeId, int status)
        {
            try
            {
                string emailTemplate = GetComfirmedEmailTemplate(permissionId,status);
                Models.Email email = new Models.Email();
                using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
                {
                    email.UpdatedDate = DateTime.Now;
                    email.UpdatedUser = 1;
                    email.CreatedDate = DateTime.Now;
                    email.CreatedUser = 1;

                    var reponsibleEmployee = dbContext.Employees.First(x => x.EmployeeId == responsibleEmployeeId);
                    var permission = dbContext.Permissions.Include(x => x.Employee).First(x => x.PermissionId == permissionId);

                    email.EmailFrom = dbContext.Parameters.Where(x => x.ParamType == 5 && x.ParamCode == "EMAIL_FROM").First().ParamDesc;
                    email.EmailTo = permission.Employee.EmailAddress;
                    email.EmailToCc = reponsibleEmployee.EmailAddress;
                    email.Subject = dbContext.Parameters.Where(x => x.ParamType == 5 && x.ParamCode == "EMAIL_SUBJECT_COMFIRMED").First().ParamDesc;

                    email.Body = emailTemplate;
                    email.IsHtml = true;
                    email.IsSend = false;
                    email.IsSuccess = null;

                    dbContext.Emails.Add(email);
                    dbContext.SaveChanges();
                }
            }
            catch
            {

            }
        }

        private string GetComfirmEmailTemplate(int permissionId)
        {
            string emailHTMLTemplate = global::System.IO.File.ReadAllText(Path.Combine(hostEnvironment.ContentRootPath, "FormTemplates/PermissionComfirmEmail.html"));

            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var permissiion = dbContext.Permissions.Include(x => x.Employee).Include(x => x.PermissionTypeNavigation).First(x => x.PermissionId == permissionId);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@NAMESURNAME@", permissiion.Employee.FullName);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@PERMISSIONTYPE@", permissiion.PermissionTypeNavigation.ParamCode);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@DESCRIPTION@", permissiion.Description);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@STARTDATE@", permissiion.StartDate.ToString("yyyy-MM-dd"));
                emailHTMLTemplate = emailHTMLTemplate.Replace("@ENDDATE@", permissiion.EndDate.ToString("yyyy-MM-dd"));
                emailHTMLTemplate = emailHTMLTemplate.Replace("@DAYCOUNT@", permissiion.DayCount.ToString());

            }
            return emailHTMLTemplate;
        }

        private string GetComfirmedEmailTemplate(int permissionId, int status)
        {
            string emailHTMLTemplate = global::System.IO.File.ReadAllText(Path.Combine(hostEnvironment.ContentRootPath, "FormTemplates/PermissionComfirmedEmail.html"));

            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var permissiion = dbContext.Permissions.Include(x => x.Employee).Include(x => x.PermissionTypeNavigation).First(x => x.PermissionId == permissionId);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@STATUS@", status == 1 ? "onaylandý" : "reddedildi");
                emailHTMLTemplate = emailHTMLTemplate.Replace("@NAMESURNAME@", permissiion.Employee.FullName);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@PERMISSIONTYPE@", permissiion.PermissionTypeNavigation.ParamCode);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@DESCRIPTION@", permissiion.Description);
                emailHTMLTemplate = emailHTMLTemplate.Replace("@STARTDATE@", permissiion.StartDate.ToString("yyyy-MM-dd"));
                emailHTMLTemplate = emailHTMLTemplate.Replace("@ENDDATE@", permissiion.EndDate.ToString("yyyy-MM-dd"));
                emailHTMLTemplate = emailHTMLTemplate.Replace("@DAYCOUNT@", permissiion.DayCount.ToString());

            }
            return emailHTMLTemplate;
        }


        public void DeletePermission(PermissionDTO permissionDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var permission = dbContext.Permissions.FirstOrDefault(x => x.PermissionId == permissionDTO.PermissionId);
                if (permission != null)
                {
                    dbContext.Permissions.Remove(permission);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class PermissionDTO
    {
        public Int32 PermissionId { get; set; }
        public Int32 EmployeeId { get; set; }
        public String EmployeeName { get; set; }
        public Int32 PermissionType { get; set; }
        public string PermissionTypeText { get; set; }
        public DateTime StartDate { get; set; }
        public Int32 DayCount { get; set; }
        public DateTime EndDate { get; set; }
        public Int32 Status { get; set; }
        public String Description { get; set; }
        public Int32? ProxyEmployee { get; set; }
        public String Duties { get; set; }
        public String Note { get; set; }
        public Int32? ApprovedWaitingEmployee { get; set; }
        public String ApprovedWaitingEmployeeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class PermissionFilter
    {
        public Int32? PermissionId { get; set; }
        public Int32? EmployeeId { get; set; }
        public Int32? PermissionType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartDate2 { get; set; }
        public Int32? DayCount { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndDate2 { get; set; }
        public Int32? Status { get; set; }
        public String Description { get; set; }
        public Int32? ProxyEmployee { get; set; }
        public String Duties { get; set; }
        public String Note { get; set; }
        public Int32? ApprovedWaitingEmployee { get; set; }
        public String ApprovedWaitingEmployeeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
        public Int32? PageType { get; set; }
    }
}
