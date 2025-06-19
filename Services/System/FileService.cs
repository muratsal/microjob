using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class FileService
    {

        public List<ItemForCombo> GetListForCombo(string search, int? value)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                List<ItemForCombo> itemComboList = new List<ItemForCombo>();
                if (!string.IsNullOrEmpty(search))
                {
                    itemComboList = dbContext.Files.
                       Where(x => x.FileOriginalName.Contains(search)).
                       Take(20).
                       ToList().
                       Select(x => new ItemForCombo
                       {
                           DisplayText = x.FileOriginalName,
                           Value = x.FileId,
                           FreeText = x.FileId.ToString()

                       }).ToList();
                }
                else if (value.HasValue && value.Value != 0)
                {
                    itemComboList = dbContext.Files.
                      Where(x => x.FileId == value).
                      ToList().
                      Select(x => new ItemForCombo
                      {
                          DisplayText = x.FileOriginalName,
                          Value = x.FileId,
                          FreeText = x.FileId.ToString()

                      }).ToList();
                }

                return itemComboList;
            }
        }

        public PageList<FileDTO> GetFileList(FileFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport, UserInfo userInfo)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Files.OrderByDescending(x => x.FileId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.FileId.HasValue) dbQuery = dbQuery.Where(x => x.FileId == filter.FileId.Value);
                    if (!string.IsNullOrEmpty(filter.Description)) dbQuery = dbQuery.Where(x => x.Description.Contains(filter.Description));
                    if (!string.IsNullOrEmpty(filter.FileOriginalName)) dbQuery = dbQuery.Where(x => x.FileOriginalName.Contains(filter.FileOriginalName));
                    if (filter.TableNo.HasValue) dbQuery = dbQuery.Where(x => x.TableNo == filter.TableNo.Value);
                    if (filter.TableReferenceId.HasValue) dbQuery = dbQuery.Where(x => x.TableReferenceId == filter.TableReferenceId.Value);
                    if (!string.IsNullOrEmpty(filter.FilePath)) dbQuery = dbQuery.Where(x => x.FilePath.Contains(filter.FilePath));
                    if (filter.FileSize.HasValue) dbQuery = dbQuery.Where(x => x.FileSize == filter.FileSize.Value);
                    if (filter.DisplayByCustomer.HasValue) dbQuery = dbQuery.Where(x => x.DisplayByCustomer == filter.DisplayByCustomer.Value);
                    if (filter.CreatedDate.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate >= filter.CreatedDate.Value);
                    if (filter.CreatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.CreatedDate <= filter.CreatedDate2.Value);
                    if (filter.CreatedUser.HasValue) dbQuery = dbQuery.Where(x => x.CreatedUser == filter.CreatedUser.Value);
                    if (filter.UpdatedDate.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate >= filter.UpdatedDate.Value);
                    if (filter.UpdatedDate2.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedDate <= filter.UpdatedDate2.Value);
                    if (filter.UpdatedUser.HasValue) dbQuery = dbQuery.Where(x => x.UpdatedUser == filter.UpdatedUser.Value);
                }

                if (userInfo.UserType == 2)
                {
                    dbQuery = dbQuery.Where(x => x.DisplayByCustomer == true);
                }

                totalCount = dbQuery.Count();

                if (queryInfo != null)
                {
                    if (!string.IsNullOrEmpty(queryInfo.OrderBy))
                    {
                        string clientOrderByName = queryInfo.OrderBy.StartsWith("-") ? queryInfo.OrderBy.Substring(1) : queryInfo.OrderBy;
                        string orderByName = typeof(Models.File).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new FileDTO
                     {
                         FileId = x.FileId,
                         Description = x.Description,
                         FileOriginalName = x.FileOriginalName,
                         TableNo = x.TableNo,
                         TableReferenceId = x.TableReferenceId,
                         FilePath = x.FilePath,
                         FileSize = x.FileSize,
                         DisplayByCustomer = x.DisplayByCustomer,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<FileDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public FileDTO SaveFile(FileDTO fileDTO, UserInfo userInfo)
        {
            Models.File file = new Models.File();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = fileDTO.FileId == 0;
                if (isNewRecord)
                {
                    file.CreatedDate = DateTime.Now;
                    file.CreatedUser = userInfo.UserId;
                }
                else
                {
                    file = dbContext.Files.First(x => x.FileId == fileDTO.FileId);

                }

                file.UpdatedDate = DateTime.Now;
                file.UpdatedUser = userInfo.UserId;


                file.Description = fileDTO.Description;
                file.FileOriginalName = fileDTO.FileOriginalName;
                file.TableNo = fileDTO.TableNo;
                file.TableReferenceId = fileDTO.TableReferenceId;
                file.FilePath = fileDTO.FilePath;
                file.FileSize = fileDTO.FileSize;
                file.DisplayByCustomer = fileDTO.DisplayByCustomer;





                if (isNewRecord)
                {
                    dbContext.Files.Add(file);
                }

                dbContext.SaveChanges();

                fileDTO.FileId = file.FileId;
            }

            return fileDTO;
        }

        public void DeleteFile(FileDTO fileDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var file = dbContext.Files.FirstOrDefault(x => x.FileId == fileDTO.FileId);
                if (file != null)
                {
                    dbContext.Files.Remove(file);
                    dbContext.SaveChanges();
                }
            }
        }


    }


    public class FileDTO
    {
        public Int32 FileId { get; set; }
        public String Description { get; set; }
        public String FileOriginalName { get; set; }
        public Int32 TableNo { get; set; }
        public Int32 TableReferenceId { get; set; }
        public String FilePath { get; set; }
        public Double FileSize { get; set; }
        public Boolean? DisplayByCustomer { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class FileFilter
    {
        public Int32? FileId { get; set; }
        public String Description { get; set; }
        public String FileOriginalName { get; set; }
        public Int32? TableNo { get; set; }
        public Int32? TableReferenceId { get; set; }
        public String FilePath { get; set; }
        public Double? FileSize { get; set; }
        public Boolean? DisplayByCustomer { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }

    public enum FileTableNo
    {
        PricingResearchItem = 1,
        BeforeLoading = 2,
        AfterLoading = 3,
    }
}
