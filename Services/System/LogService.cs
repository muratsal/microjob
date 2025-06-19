using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class LogService
    {
        public PageList<LogDTO> GetLogList(LogFilter filter, NanoGo.Shared.QueryInfo queryInfo, bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Logs.OrderByDescending(x => x.LogId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.LogId.HasValue) dbQuery = dbQuery.Where(x => x.LogId == filter.LogId.Value);
                    if (filter.LogType.HasValue) dbQuery = dbQuery.Where(x => x.LogType == filter.LogType.Value);
                    if (!string.IsNullOrEmpty(filter.Source)) dbQuery = dbQuery.Where(x => x.Source.Contains(filter.Source));
                    if (!string.IsNullOrEmpty(filter.Message)) dbQuery = dbQuery.Where(x => x.Message.Contains(filter.Message));
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
                        string orderByName = typeof(Models.Log).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new LogDTO
                     {
                         LogId = x.LogId,
                         LogType = x.LogType,
                         Source = x.Source,
                         Message = x.Message,
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

                return new PageList<LogDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public LogDTO SaveLog(LogDTO logDTO, UserInfo userInfo)
        {
            Models.Log log = new Models.Log();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = logDTO.LogId == 0;
                if (isNewRecord)
                {
                    log.CreatedDate = DateTime.Now;
                    log.CreatedUser = userInfo.UserId;
                }
                else
                {
                    log = dbContext.Logs.First(x => x.LogId == logDTO.LogId);

                }

                log.UpdatedDate = DateTime.Now;
                log.UpdatedUser = userInfo.UserId;

                log.LogType = logDTO.LogType;
                log.Source = logDTO.Source;
                log.Message = logDTO.Message;
                log.FreeText1 = logDTO.FreeText1;
                log.FreeText2 = logDTO.FreeText2;
                log.FreeText3 = logDTO.FreeText3;

                if (isNewRecord)
                {
                    dbContext.Logs.Add(log);
                }

                dbContext.SaveChanges();

                logDTO.LogId = log.LogId;
            }

            return logDTO;
        }


        public void DeleteLog(LogDTO logDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var log = dbContext.Logs.FirstOrDefault(x => x.LogId == logDTO.LogId);
                if (log != null)
                {
                    dbContext.Logs.Remove(log);
                    dbContext.SaveChanges();
                }
            }
        }

        public void AddLog(string source,
                            string message,
                            string freeText1 = "",
                            string freeText2 = "",
                            string freeText3 = "",
                            LOG_TYPES logType = LOG_TYPES.ERROR,
                            int userId = 1)
        {
            Models.Log log = new Models.Log();

            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                log.CreatedDate = DateTime.Now;
                log.CreatedUser = userId;
                log.UpdatedDate = DateTime.Now;
                log.UpdatedUser = userId;

                log.LogType = (int)logType;
                log.Source = source;
                log.Message = message;
                log.FreeText1 = freeText1;
                log.FreeText2 = freeText2;
                log.FreeText3 = freeText3;

                dbContext.Logs.Add(log);

                dbContext.SaveChanges();
            }
        }
    }

    public enum LOG_TYPES
    {
        ERROR = 1,
        WARNING = 2,
        INFO = 3
    }

    public class LogDTO
    {
        public Int32 LogId { get; set; }
        public Int32 LogType { get; set; }
        public String Source { get; set; }
        public String Message { get; set; }
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

    public class LogFilter
    {
        public Int32? LogId { get; set; }
        public Int32? LogType { get; set; }
        public String Source { get; set; }
        public String Message { get; set; }
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
