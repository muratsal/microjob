using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class InventoryService
    {

        public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.Inventories.
                       Where(x => (x.SerieNo + " " + x.Model).Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.SerieNo.ToString() + " " + x.Model.ToString(),
                           Value = x.InventoryId,
                           FreeText = x.InventoryId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.Inventories.
                      Where(x => x.InventoryId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.SerieNo.ToString() + " " + x.Model.ToString(),
                          Value = x.InventoryId,
                          FreeText = x.InventoryId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<InventoryDTO> GetInventoryList(InventoryFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Inventories.Include(x => x.Company).Include(x => x.InventoryTypeNavigation).OrderByDescending(x => x.InventoryId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.InventoryId.HasValue) dbQuery = dbQuery.Where(x => x.InventoryId == filter.InventoryId.Value);
                    if (!string.IsNullOrEmpty(filter.InventoryName)) dbQuery = dbQuery.Where(x => x.InventoryName.Contains(filter.InventoryName));
                    if (!string.IsNullOrEmpty(filter.SerieNo)) dbQuery = dbQuery.Where(x => x.SerieNo.Contains(filter.SerieNo));
                    if (!string.IsNullOrEmpty(filter.Model)) dbQuery = dbQuery.Where(x => x.Model.Contains(filter.Model));
                    if (filter.InventoryType.HasValue) dbQuery = dbQuery.Where(x => x.InventoryType == filter.InventoryType.Value);
                    if (filter.IsActive.HasValue) dbQuery = dbQuery.Where(x => x.IsActive == filter.IsActive.Value);
                    if (!string.IsNullOrEmpty(filter.Picture)) dbQuery = dbQuery.Where(x => x.Picture.Contains(filter.Picture));
                    if (!string.IsNullOrEmpty(filter.Description)) dbQuery = dbQuery.Where(x => x.Description.Contains(filter.Description));
                    if (!string.IsNullOrEmpty(filter.Note)) dbQuery = dbQuery.Where(x => x.Note.Contains(filter.Note));
                    if (filter.CompanyId.HasValue) dbQuery = dbQuery.Where(x => x.CompanyId == filter.CompanyId.Value);
                    if (filter.InventoryDate.HasValue) dbQuery = dbQuery.Where(x => x.InventoryDate >= filter.InventoryDate.Value);
                    if (filter.InventoryDate2.HasValue) dbQuery = dbQuery.Where(x => x.InventoryDate <= filter.InventoryDate2.Value);
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
                        string orderByName = typeof(Models.Inventory).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new InventoryDTO
                     {
                         InventoryId = x.InventoryId,
                         InventoryName = x.InventoryName,
                         SerieNo = x.SerieNo,
                         Model = x.Model,
                         InventoryType = x.InventoryType,
                         InventoryTypeText = x.InventoryTypeNavigation != null ? x.InventoryTypeNavigation.ParamCode : "",
                         IsActive = x.IsActive,
                         Picture = x.Picture,
                         Description = x.Description,
                         Note = x.Note,
                         CompanyId = x.CompanyId,
                         CompanyName = x.Company.CompanyName,
                         InventoryDate = x.InventoryDate,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         LastTransaction = dbContext.InventoryTransactions.Include(y => y.Employee)
                         .OrderByDescending(y => y.InventoryTransId)
                         .Where(y => y.InventoryId == x.InventoryId)
                         .Select(y => new InventoryTransactionDTO
                         {
                             TransType = y.TransType,
                             EmployeeId = y.EmployeeId,
                             EmployeeName = y.Employee !=null ? y.Employee.FullName :""
                         }).FirstOrDefault(),
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                data.ForEach(x =>
                {
                    x.TransactionEmployeeName = x.LastTransaction != null ? x.LastTransaction.EmployeeName:"";
                    x.TransType = x.LastTransaction != null ? x.LastTransaction.TransType : (int?) null;
                    if(x.TransType.HasValue)
                    {
                        if (x.TransType.Value == 1) x.TransTypeText = "Envanter Giriş";
                        if (x.TransType.Value == 2) x.TransTypeText = "Envanter Çıkış";
                        if (x.TransType.Value == 3) x.TransTypeText = "Zimmet Verildi";
                        if (x.TransType.Value == 4) x.TransTypeText = "Zimmet Geri Alındı";
                    }
                });

                return new PageList<InventoryDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public InventoryDTO SaveInventory(InventoryDTO ınventoryDTO, UserInfo userInfo)
        {
            Models.Inventory ınventory = new Models.Inventory();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = ınventoryDTO.InventoryId == 0;
                if (isNewRecord)
                {
                    ınventory.CreatedDate = DateTime.Now;
                    ınventory.CreatedUser = userInfo.UserId;
                }
                else
                {
                    ınventory = dbContext.Inventories.First(x => x.InventoryId == ınventoryDTO.InventoryId);

                }

                ınventory.UpdatedDate = DateTime.Now;
                ınventory.UpdatedUser = userInfo.UserId;


                ınventory.InventoryName = ınventoryDTO.InventoryName;
                ınventory.SerieNo = ınventoryDTO.SerieNo;
                ınventory.Model = ınventoryDTO.Model;
                ınventory.InventoryType = ınventoryDTO.InventoryType;
                ınventory.IsActive = ınventoryDTO.IsActive;
                ınventory.Picture = ınventoryDTO.Picture;
                ınventory.Description = ınventoryDTO.Description;
                ınventory.Note = ınventoryDTO.Note;
                ınventory.CompanyId = ınventoryDTO.CompanyId;
                ınventory.InventoryDate = ınventoryDTO.InventoryDate;





                if (isNewRecord)
                {
                    dbContext.Inventories.Add(ınventory);
                }

                dbContext.SaveChanges();

                ınventoryDTO.InventoryId = ınventory.InventoryId;
            }

            return ınventoryDTO;
        }


        public void DeleteInventory(InventoryDTO ınventoryDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var ınventory = dbContext.Inventories.FirstOrDefault(x => x.InventoryId == ınventoryDTO.InventoryId);
                if (ınventory != null)
                {
                    dbContext.Inventories.Remove(ınventory);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class InventoryDTO
    {
        public Int32 InventoryId { get; set; }
        public String InventoryName { get; set; }
        public String SerieNo { get; set; }
        public String Model { get; set; }
        public Int32 InventoryType { get; set; }
        public String InventoryTypeText { get; set; }
        public Boolean IsActive { get; set; }
        public String Picture { get; set; }
        public String Description { get; set; }
        public String Note { get; set; }
        public Int32 CompanyId { get; set; }
        public String CompanyName { get; set; }
        public DateTime InventoryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }

        public String  TransactionEmployeeName { get; set; }
        public Int32? TransType { get; set; }
        public String TransTypeText { get; set; }

        public InventoryTransactionDTO LastTransaction { get; set; }
    }

    public class InventoryFilter
    {
        public Int32? InventoryId { get; set; }
        public String InventoryName { get; set; }
        public String SerieNo { get; set; }
        public String Model { get; set; }
        public Int32? InventoryType { get; set; }
        public Boolean? IsActive { get; set; }
        public String Picture { get; set; }
        public String Description { get; set; }
        public String Note { get; set; }
        public Int32? CompanyId { get; set; }
        public DateTime? InventoryDate { get; set; }
        public DateTime? InventoryDate2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
