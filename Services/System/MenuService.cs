using NanoGo.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Services.System
{
    public class MenuService
    {
        public PageList<MenuDTO> GetMenuList(MenuFilter filter, NanoGo.Shared.QueryInfo queryInfo,bool isExport)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var dbQuery = dbContext.Menus.OrderByDescending(x => x.MenuId).Where(x => 1 == 1);
                int totalCount = 0;

                if (filter != null)
                {
                    if (filter.MenuId.HasValue) dbQuery = dbQuery.Where(x => x.MenuId == filter.MenuId.Value);
if (!string.IsNullOrEmpty(filter.MenuIcon)) dbQuery = dbQuery.Where(x => x.MenuIcon.Contains(filter.MenuIcon));
if (!string.IsNullOrEmpty(filter.Name)) dbQuery = dbQuery.Where(x => x.Name.Contains(filter.Name));
if (!string.IsNullOrEmpty(filter.State)) dbQuery = dbQuery.Where(x => x.State.Contains(filter.State));
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
                        string orderByName = typeof(Models.Menu).GetProperties().Where(x => x.Name.ToUpper() == clientOrderByName.ToUpper()).First().Name;

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
                     .Select(x => new MenuDTO
                     {
                        MenuId = x.MenuId,
MenuIcon = x.MenuIcon,
Name = x.Name,
State = x.State,
CreatedDate = x.CreatedDate,
CreatedUser = x.CreatedUser,
UpdatedDate = x.UpdatedDate,
UpdatedUser = x.UpdatedUser,
                         CreatedUserText = dbContext.Users.First(y => y.UserId == x.CreatedUser).UserName,
                         UpdatedUserText = dbContext.Users.First(y => y.UserId == x.UpdatedUser).UserName
                     }).ToList();

                return new PageList<MenuDTO> { Data = data, TotalCount = totalCount };

            }
        }

        public MenuDTO SaveMenu(MenuDTO menuDTO,  UserInfo userInfo)
        {
            Models.Menu menu = new Models.Menu();
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                bool isNewRecord = menuDTO.MenuId == 0;
                if (isNewRecord)
                {
                    menu.CreatedDate = DateTime.Now;
                    menu.CreatedUser = userInfo.UserId;
                }
                else
                {
                    menu = dbContext.Menus.First(x => x.MenuId == menuDTO.MenuId);
                   
                }

                 menu.UpdatedDate = DateTime.Now;
                 menu.UpdatedUser = userInfo.UserId;

                
menu.MenuIcon = menuDTO.MenuIcon ;
menu.Name = menuDTO.Name ;
menu.State = menuDTO.State ;





                if (isNewRecord)
                {
                    dbContext.Menus.Add(menu);
                }

                dbContext.SaveChanges();

                menuDTO.MenuId = menu.MenuId;
            }

            return menuDTO;
        }


        public void DeleteMenu(MenuDTO menuDTO)
        {
            using (Models.NanoGoContext dbContext = new Models.NanoGoContext())
            {
                var menu = dbContext.Menus.FirstOrDefault(x => x.MenuId == menuDTO.MenuId);
                if (menu != null)
                {
                    dbContext.Menus.Remove(menu);
                    dbContext.SaveChanges();
                }
            }
        }
    }


    public class MenuDTO
    {
       public Int32 MenuId { get; set; }
public String MenuIcon { get; set; }
public String Name { get; set; }
public String State { get; set; }
public DateTime? CreatedDate { get; set; }
public Int32? CreatedUser { get; set; }
public DateTime? UpdatedDate { get; set; }
public Int32? UpdatedUser { get; set; }
       public String CreatedUserText  {get;set;}
       public String UpdatedUserText  {get;set;}
    }

    public class MenuFilter
    {
        public Int32? MenuId { get; set; }
public String MenuIcon { get; set; }
public String Name { get; set; }
public String State { get; set; }
public DateTime? CreatedDate { get; set; }
public DateTime? CreatedDate2 { get; set; }
public Int32? CreatedUser { get; set; }
public DateTime? UpdatedDate { get; set; }
public DateTime? UpdatedDate2 { get; set; }
public Int32? UpdatedUser { get; set; }
    }
}
