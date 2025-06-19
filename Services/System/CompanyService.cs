using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class CompanyService
    {

        public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.Companies.
                       Where(x => x.CompanyName.Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.CompanyName.ToString(),
                           Value = x.CompanyId,
                           FreeText = x.CompanyId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.Companies.
                      Where(x => x.CompanyId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.CompanyName.ToString(),
                          Value = x.CompanyId,
                          FreeText = x.CompanyId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<CompanyDTO> GetCompanyList(CompanyFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Companies.OrderByDescending(x => x.CompanyId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.CompanyId.HasValue) dbQuery = dbQuery.Where(x => x.CompanyId == filter.CompanyId.Value);
                    if (!string.IsNullOrEmpty(filter.CompanyName)) dbQuery = dbQuery.Where(x => x.CompanyName.Contains(filter.CompanyName));
                    if (!string.IsNullOrEmpty(filter.Logo)) dbQuery = dbQuery.Where(x => x.Logo.Contains(filter.Logo));
                    if (!string.IsNullOrEmpty(filter.Address)) dbQuery = dbQuery.Where(x => x.Address.Contains(filter.Address));
                    if (!string.IsNullOrEmpty(filter.Phone)) dbQuery = dbQuery.Where(x => x.Phone.Contains(filter.Phone));
                    if (!string.IsNullOrEmpty(filter.Email)) dbQuery = dbQuery.Where(x => x.Email.Contains(filter.Email));
                    if (!string.IsNullOrEmpty(filter.Website)) dbQuery = dbQuery.Where(x => x.Website.Contains(filter.Website));
                    if (!string.IsNullOrEmpty(filter.TaxAuthority)) dbQuery = dbQuery.Where(x => x.TaxAuthority.Contains(filter.TaxAuthority));
                    if (!string.IsNullOrEmpty(filter.TaxNumber)) dbQuery = dbQuery.Where(x => x.TaxNumber.Contains(filter.TaxNumber));
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
                        string orderByName = typeof(Models.Company).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new CompanyDTO
                     {
                         CompanyId = x.CompanyId,
                         CompanyName = x.CompanyName,
                         Logo = x.Logo,
                         Address = x.Address,
                         Phone = x.Phone,
                         Email = x.Email,
                         Website = x.Website,
                         TaxAuthority = x.TaxAuthority,
                         TaxNumber = x.TaxNumber,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<CompanyDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public CompanyDTO SaveCompany(CompanyDTO companyDTO, UserInfo userInfo)
        {
            Models.Company company = new Models.Company();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = companyDTO.CompanyId == 0;
                if (isNewRecord)
                {
                    company.CreatedDate = DateTime.Now;
                    company.CreatedUser = userInfo.UserId;
                }
                else
                {
                    company = dbContext.Companies.First(x => x.CompanyId == companyDTO.CompanyId);

                }

                company.UpdatedDate = DateTime.Now;
                company.UpdatedUser = userInfo.UserId;


                company.CompanyName = companyDTO.CompanyName;
                company.Logo = companyDTO.Logo;
                company.Address = companyDTO.Address;
                company.Phone = companyDTO.Phone;
                company.Email = companyDTO.Email;
                company.Website = companyDTO.Website;
                company.TaxAuthority = companyDTO.TaxAuthority;
                company.TaxNumber = companyDTO.TaxNumber;





                if (isNewRecord)
                {
                    dbContext.Companies.Add(company);
                }

                dbContext.SaveChanges();

                companyDTO.CompanyId = company.CompanyId;
            }

            return companyDTO;
        }


        public void DeleteCompany(CompanyDTO companyDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var company = dbContext.Companies.FirstOrDefault(x => x.CompanyId == companyDTO.CompanyId);
                if (company != null)
                {
                    dbContext.Companies.Remove(company);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class CompanyDTO
    {
        public Int32 CompanyId { get; set; }
        public String CompanyName { get; set; }
        public String Logo { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
        public String TaxAuthority { get; set; }
        public String TaxNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class CompanyFilter
    {
        public Int32? CompanyId { get; set; }
        public String CompanyName { get; set; }
        public String Logo { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
        public String TaxAuthority { get; set; }
        public String TaxNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
