using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class CounterService
    {

       public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.Counters.
                       Where(x => x.CounterName.Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.CounterName,
                           Value = x.CounterId,
                           FreeText = x.CounterId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.Counters.
                      Where(x => x.CounterId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.CounterName,
                           Value = x.CounterId,
                           FreeText = x.CounterId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<CounterDTO> GetCounterList(CounterFilter filter, NanoGo.Shared.QueryInfo queryInfo,bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Counters.OrderByDescending(x => x.CounterId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.CounterId.HasValue) dbQuery = dbQuery.Where(x => x.CounterId == filter.CounterId.Value);
if (!string.IsNullOrEmpty(filter.CounterName)) dbQuery = dbQuery.Where(x => x.CounterName.Contains(filter.CounterName));
if (filter.CurrentValue.HasValue) dbQuery = dbQuery.Where(x => x.CurrentValue == filter.CurrentValue.Value);
if (!string.IsNullOrEmpty(filter.Prefix)) dbQuery = dbQuery.Where(x => x.Prefix.Contains(filter.Prefix));
if (filter.AddYear.HasValue) dbQuery = dbQuery.Where(x => x.AddYear == filter.AddYear.Value);
if (filter.PaddingCount.HasValue) dbQuery = dbQuery.Where(x => x.PaddingCount == filter.PaddingCount.Value);
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
                        string orderByName = typeof(Models.Counter).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new CounterDTO
                     {
                        CounterId = x.CounterId,
CounterName = x.CounterName,
CurrentValue = x.CurrentValue,
Prefix = x.Prefix,
AddYear = x.AddYear,
PaddingCount = x.PaddingCount,
CreatedDate = x.CreatedDate,
CreatedUser = x.CreatedUser,
UpdatedDate = x.UpdatedDate,
UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<CounterDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public CounterDTO SaveCounter(CounterDTO counterDTO,  UserInfo userInfo)
        {
            Models.Counter counter = new Models.Counter();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = counterDTO.CounterId == 0;
                if (isNewRecord)
                {
                    counter.CreatedDate = DateTime.Now;
                    counter.CreatedUser = userInfo.UserId;
                }
                else
                {
                    counter = dbContext.Counters.First(x => x.CounterId == counterDTO.CounterId);
                   
                }

                 counter.UpdatedDate = DateTime.Now;
                 counter.UpdatedUser = userInfo.UserId;

                
counter.CounterName = counterDTO.CounterName ;
counter.CurrentValue = counterDTO.CurrentValue ;
counter.Prefix = counterDTO.Prefix ;
counter.AddYear = counterDTO.AddYear ;
counter.PaddingCount = counterDTO.PaddingCount ;





                if (isNewRecord)
                {
                    dbContext.Counters.Add(counter);
                }

                dbContext.SaveChanges();

                counterDTO.CounterId = counter.CounterId;
            }

            return counterDTO;
        }


        public void DeleteCounter(CounterDTO counterDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var counter = dbContext.Counters.FirstOrDefault(x => x.CounterId == counterDTO.CounterId);
                if (counter != null)
                {
                    dbContext.Counters.Remove(counter);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class CounterDTO
    {
       public Int32 CounterId { get; set; }
public String CounterName { get; set; }
public Int32 CurrentValue { get; set; }
public String Prefix { get; set; }
public Boolean AddYear { get; set; }
public Int32 PaddingCount { get; set; }
public DateTime CreatedDate { get; set; }
public Int32 CreatedUser { get; set; }
public DateTime UpdatedDate { get; set; }
public Int32 UpdatedUser { get; set; }
       public String CreatedUserText  {get;set;}
       public String UpdatedUserText  {get;set;}
    }

    public class CounterFilter
    {
        public Int32? CounterId { get; set; }
public String CounterName { get; set; }
public Int32? CurrentValue { get; set; }
public String Prefix { get; set; }
public Boolean? AddYear { get; set; }
public Int32? PaddingCount { get; set; }
public DateTime? CreatedDate { get; set; }
public DateTime? CreatedDate2 { get; set; }
public Int32? CreatedUser { get; set; }
public DateTime? UpdatedDate { get; set; }
public DateTime? UpdatedDate2 { get; set; }
public Int32? UpdatedUser { get; set; }
    }
}
