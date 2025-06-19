using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class InventoryTransactionService
    {

        public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.InventoryTransactions.
                       Where(x => x.InventoryTransId.ToString().Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.InventoryTransId.ToString(),
                           Value = x.InventoryTransId,
                           FreeText = x.InventoryTransId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.InventoryTransactions.
                      Where(x => x.InventoryTransId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.InventoryTransId.ToString(),
                          Value = x.InventoryTransId,
                          FreeText = x.InventoryTransId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<InventoryTransactionDTO> GetInventoryTransactionList(InventoryTransactionFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.InventoryTransactions
                    .Include(x=> x.Inventory)
                    .Include(x => x.Employee)
                    .OrderByDescending(x => x.InventoryTransId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.InventoryTransId.HasValue) dbQuery = dbQuery.Where(x => x.InventoryTransId == filter.InventoryTransId.Value);
                    if (filter.InventoryId.HasValue) dbQuery = dbQuery.Where(x => x.InventoryId == filter.InventoryId.Value);
                    if (filter.TransDate.HasValue) dbQuery = dbQuery.Where(x => x.TransDate >= filter.TransDate.Value);
                    if (filter.TransDate2.HasValue) dbQuery = dbQuery.Where(x => x.TransDate <= filter.TransDate2.Value);
                    if (filter.EmployeeId.HasValue) dbQuery = dbQuery.Where(x => x.EmployeeId == filter.EmployeeId.Value);
                    if (filter.TransType.HasValue) dbQuery = dbQuery.Where(x => x.TransType == filter.TransType.Value);
                    if (!string.IsNullOrEmpty(filter.Note)) dbQuery = dbQuery.Where(x => x.Note.Contains(filter.Note));
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
                        string orderByName = typeof(Models.InventoryTransaction).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new InventoryTransactionDTO
                     {
                         InventoryTransId = x.InventoryTransId,
                         InventoryId = x.InventoryId,
                         InventorySerieNo = x.Inventory.SerieNo,
                         TransDate = x.TransDate,
                         EmployeeId = x.EmployeeId,
                         EmployeeName = x.Employee !=null ? x.Employee.FullName :"",
                         TransType = x.TransType,
                         Note = x.Note,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<InventoryTransactionDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public InventoryTransactionDTO SaveInventoryTransaction(InventoryTransactionDTO ınventoryTransactionDTO, UserInfo userInfo)
        {
            Models.InventoryTransaction ınventoryTransaction = new Models.InventoryTransaction();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = ınventoryTransactionDTO.InventoryTransId == 0;
                if (isNewRecord)
                {
                    ınventoryTransaction.CreatedDate = DateTime.Now;
                    ınventoryTransaction.CreatedUser = userInfo.UserId;
                }
                else
                {
                    ınventoryTransaction = dbContext.InventoryTransactions.First(x => x.InventoryTransId == ınventoryTransactionDTO.InventoryTransId);

                }

                ınventoryTransaction.UpdatedDate = DateTime.Now;
                ınventoryTransaction.UpdatedUser = userInfo.UserId;


                ınventoryTransaction.InventoryId = ınventoryTransactionDTO.InventoryId;
                ınventoryTransaction.TransDate = ınventoryTransactionDTO.TransDate;
                ınventoryTransaction.EmployeeId = ınventoryTransactionDTO.EmployeeId;
                ınventoryTransaction.TransType = ınventoryTransactionDTO.TransType;
                ınventoryTransaction.Note = ınventoryTransactionDTO.Note;





                if (isNewRecord)
                {
                    dbContext.InventoryTransactions.Add(ınventoryTransaction);
                }

                dbContext.SaveChanges();

                ınventoryTransactionDTO.InventoryTransId = ınventoryTransaction.InventoryTransId;
            }

            return ınventoryTransactionDTO;
        }


        public void DeleteInventoryTransaction(InventoryTransactionDTO ınventoryTransactionDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var ınventoryTransaction = dbContext.InventoryTransactions.FirstOrDefault(x => x.InventoryTransId == ınventoryTransactionDTO.InventoryTransId);
                if (ınventoryTransaction != null)
                {
                    dbContext.InventoryTransactions.Remove(ınventoryTransaction);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class InventoryTransactionDTO
    {
        public Int32 InventoryTransId { get; set; }
        public Int32 InventoryId { get; set; }
        public String InventorySerieNo { get; set; }
        public DateTime TransDate { get; set; }
        public Int32? EmployeeId { get; set; }
        public String EmployeeName { get; set; }
        public Int32 TransType { get; set; }
        public String Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
       
    }

    public class InventoryTransactionFilter
    {
        public Int32? InventoryTransId { get; set; }
        public Int32? InventoryId { get; set; }
        public String InventorySerieNo { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? TransDate2 { get; set; }
        public Int32? EmployeeId { get; set; }
        public Int32? TransType { get; set; }
        public String Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }

    }
}
