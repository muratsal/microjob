using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class ParameterService
    {
        public PageList<ParameterDTO> GetParameterList(ParameterFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Parameters.OrderByDescending(x => x.ParamId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.ParamId.HasValue) dbQuery = dbQuery.Where(x => x.ParamId == filter.ParamId.Value);
                    if (filter.ParamType.HasValue) dbQuery = dbQuery.Where(x => x.ParamType == filter.ParamType.Value);
                    if (!string.IsNullOrEmpty(filter.ParamCode)) dbQuery = dbQuery.Where(x => x.ParamCode.Contains(filter.ParamCode));
                    if (!string.IsNullOrEmpty(filter.ParamDesc)) dbQuery = dbQuery.Where(x => x.ParamDesc.Contains(filter.ParamDesc));
                    if (filter.ParentId.HasValue) dbQuery = dbQuery.Where(x => x.ParentId == filter.ParentId.Value);
                    if (!string.IsNullOrEmpty(filter.FreeText1)) dbQuery = dbQuery.Where(x => x.FreeText1.Contains(filter.FreeText1));
                    if (!string.IsNullOrEmpty(filter.FreeText2)) dbQuery = dbQuery.Where(x => x.FreeText2.Contains(filter.FreeText2));
                    if (!string.IsNullOrEmpty(filter.FreeText3)) dbQuery = dbQuery.Where(x => x.FreeText3.Contains(filter.FreeText3));
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
                        string orderByName = typeof(Models.Parameter).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new ParameterDTO
                     {
                         ParamId = x.ParamId,
                         ParamType = x.ParamType,
                         ParamCode = x.ParamCode,
                         ParamDesc = x.ParamDesc,
                         ParentId = x.ParentId,
                         FreeText1 = x.FreeText1,
                         FreeText2 = x.FreeText2,
                         FreeText3 = x.FreeText3,
                         CreatedDate = x.CreatedDate,
                         CreatedUser = x.CreatedUser,
                         UpdatedDate = x.UpdatedDate,
                         UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<ParameterDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public ParameterDTO SaveParameter(ParameterDTO parameterDTO, UserInfo userInfo)
        {
            Models.Parameter parameter = new Models.Parameter();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = parameterDTO.ParamId == 0;
                if (isNewRecord)
                {
                    parameter.CreatedDate = DateTime.Now;
                    parameter.CreatedUser = userInfo.UserId;
                }
                else
                {
                    parameter = dbContext.Parameters.First(x => x.ParamId == parameterDTO.ParamId);

                }

                parameter.UpdatedDate = DateTime.Now;
                parameter.UpdatedUser = userInfo.UserId;


                parameter.ParamType = parameterDTO.ParamType;
                parameter.ParamCode = parameterDTO.ParamCode;
                parameter.ParamDesc = parameterDTO.ParamDesc;
                parameter.ParentId = parameterDTO.ParentId;
                parameter.FreeText1 = parameterDTO.FreeText1;
                parameter.FreeText2 = parameterDTO.FreeText2;
                parameter.FreeText3 = parameterDTO.FreeText3;





                if (isNewRecord)
                {
                    dbContext.Parameters.Add(parameter);
                }

                dbContext.SaveChanges();

                parameterDTO.ParamId = parameter.ParamId;
            }

            return parameterDTO;
        }


        public void DeleteParameter(ParameterDTO parameterDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var parameter = dbContext.Parameters.FirstOrDefault(x => x.ParamId == parameterDTO.ParamId);
                if (parameter != null)
                {
                    dbContext.Parameters.Remove(parameter);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class ParameterDTO
    {
        public Int32 ParamId { get; set; }
        public Int32 ParamType { get; set; }
        public String ParamCode { get; set; }
        public String ParamDesc { get; set; }
        public Int32? ParentId { get; set; }
        public String FreeText1 { get; set; }
        public String FreeText2 { get; set; }
        public String FreeText3 { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Int32 UpdatedUser { get; set; }
        public String CreatedUserText { get; set; }
        public String UpdatedUserText { get; set; }
    }

    public class ParameterFilter
    {
        public Int32? ParamId { get; set; }
        public Int32? ParamType { get; set; }
        public String ParamCode { get; set; }
        public String ParamDesc { get; set; }
        public Int32? ParentId { get; set; }
        public String FreeText1 { get; set; }
        public String FreeText2 { get; set; }
        public String FreeText3 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CreatedDate2 { get; set; }
        public Int32? CreatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? UpdatedDate2 { get; set; }
        public Int32? UpdatedUser { get; set; }
    }
}
