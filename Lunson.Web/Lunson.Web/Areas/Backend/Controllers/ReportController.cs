using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lunson.BLL.Services;
using Lunson.Web.Models;
using System.Web.UI.WebControls;
using Pharos.Framework;
using Lunson.Web.Areas.Backend.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using NPOI.SS.Util;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class ReportController : BaseController
    {
        ReportService rsv = new ReportService();

        [PermissionCheck("view", "index")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 购票详细报表数据
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [PermissionCheck("view", "index")]
        [HttpPost]
        public ActionResult DetailReportData(DateTime? fromTime, DateTime? endTime, int page = 1, int rows = 20)
        {
            var records = rsv.DetailReportData<DetailReportVM>(fromTime, endTime);
            var pageList = records.Skip((page - 1) * rows).Take(rows);

            var list = (new int[] { 1 }).Select(x => new { CurPrice = "交易额统计", AllPrice = records.Sum(a => (decimal)a.AllPrice) }).ToList();

            return Json(new { total = records.Count(), rows = pageList, footer = list });
        }

        [PermissionCheck("view", "Statistics")]
        public ActionResult Statistics()
        {
            return View();
        }

        /// <summary>
        /// 购票统计报表数据
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [PermissionCheck("view", "Statistics")]
        [HttpPost]
        public ActionResult StatisticsData(DateTime? fromTime, DateTime? endTime)
        {
            var ids = Request.Params["ids[]"].ToEmptyString().Split(',').Where(a => a.IsNullOrTrimEmpty() == false).Select(a => a.Trim()).ToList();
            var records = rsv.StatisticsData<StatisticVM>(fromTime, endTime, ids);

            var list = (new int[] { 1 }).Select(x => new { Name = "合计张数", Num = records.Sum(a => a.Num), SalePrice = "合计收入", Income = records.Sum(a => (decimal)a.Income) }).ToList();

            return Json(new { total = records.Count(), rows = records, footer = list });
        }

        /// <summary>
        /// 导出xls报表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="titleText"></param>
        /// <param name="filterText"></param>
        /// <returns></returns>
        public MemoryStream RenderToExcel<T>(List<T> datas, string titleText, string filterText)
        {
            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("报表数据");
            IRow title = sheet.CreateRow(0);
            IRow filterRow = sheet.CreateRow(1);
            IRow header = sheet.CreateRow(2);

            int rowIndex = 3, piIndex = 0;//rowIndex为数据起始索引，piIndex为属性值（每行单元格）索引
            Type type = typeof(T);
            var pis = type.GetProperties();
            int pisLen = pis.Length;
            PropertyInfo pi = null;
            string displayName = string.Empty;
            while (piIndex < pisLen)
            {
                title.CreateCell(piIndex);
                filterRow.CreateCell(piIndex);

                sheet.SetColumnWidth(piIndex, 20 * 256);

                pi = pis[piIndex];
                displayName = (pi.GetCustomAttributes(typeof(DisplayAttribute), false)[0] as DisplayAttribute).Name;//获取列字段中文名
                if (!displayName.Equals(string.Empty))
                {
                    try
                    {
                        header.CreateCell(piIndex).SetCellValue(displayName);
                    }
                    catch
                    {
                        header.CreateCell(piIndex).SetCellValue("");
                    }
                }
                piIndex++;
            }

            title.GetCell(0).SetCellValue(titleText);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, pisLen - 1));//合并单元格，参数为（起始行，结束行，起始列，结束列）
            filterRow.GetCell(0).SetCellValue("筛选条件:" + filterText);

            ICellStyle style1 = workbook.CreateCellStyle();
            IFont font1 = workbook.CreateFont();
            font1.Boldweight = short.MaxValue;
            font1.FontHeight = 15 * 20;
            style1.SetFont(font1);
            title.GetCell(0).CellStyle = style1;

            ICellStyle style2 = workbook.CreateCellStyle();
            IFont font2 = workbook.CreateFont();
            font2.Boldweight = short.MaxValue;
            style2.SetFont(font2);
            filterRow.GetCell(0).CellStyle = style2;

            foreach (T data in datas)
            {
                piIndex = 0;
                IRow dataRow = sheet.CreateRow(rowIndex);
                while (piIndex < pisLen)
                {
                    pi = pis[piIndex];
                    try
                    {
                        dataRow.CreateCell(piIndex).SetCellValue(pi.GetValue(data, null).ToString());
                    }
                    catch
                    {
                        dataRow.CreateCell(piIndex).SetCellValue("");
                    }
                    piIndex++;
                }
                rowIndex++;
            }

            workbook.Write(ms);

            return ms;
        }

        /// <summary>
        /// 导出购票日详细报表
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [PermissionCheck("export", "index")]
        public FileResult ExportDetailReportData(DateTime? fromTime, DateTime? endTime)
        {
            var records = rsv.DetailReportData<DetailReportVM>(fromTime, endTime);
            var title = "南顺鳄鱼园 卖出交易详细报表";
            var filter = fromTime.ToString() + "到" + endTime.ToString();
            var memoryStream = RenderToExcel(records, title, filter);

            var fileName = DateTime.Now.ToString("yyyyMMdd") + "卖出交易详细报表.xls";

            var temp = memoryStream.GetBuffer();

            memoryStream.Close();
            memoryStream.Dispose();


            return File(temp, "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// 导出购票统计报表
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [PermissionCheck("export", "Statistics")]
        public FileResult ExportStatisticsData(DateTime? fromTime, DateTime? endTime)
        {
            var ids = Request.Params["ids[]"].ToEmptyString().Split(',').Where(a => a.IsNullOrTrimEmpty() == false).Select(a => a.Trim()).ToList();
            var records = rsv.StatisticsData<StatisticVM>(fromTime, endTime, ids);
            var title = "南顺鳄鱼园 交易完成统计报表";
            var filter = fromTime.ToString() + "到" + endTime.ToString();
            //var memoryStream = RenderToExcel(records, title, filter);


            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("报表数据");
            IRow titleRow = sheet.CreateRow(0);
            IRow filterRow = sheet.CreateRow(1);
            IRow header = sheet.CreateRow(2);

            int rowIndex = 3, piIndex = 0;//rowIndex为数据起始索引，piIndex为属性值（每行单元格）索引
            Type type = typeof(StatisticVM);
            var pis = type.GetProperties();
            int pisLen = pis.Length;
            PropertyInfo pi = null;
            string displayName = string.Empty;
            while (piIndex < pisLen)
            {
                titleRow.CreateCell(piIndex);
                filterRow.CreateCell(piIndex);

                sheet.SetColumnWidth(piIndex, 20 * 256);

                pi = pis[piIndex];
                displayName = (pi.GetCustomAttributes(typeof(DisplayAttribute), false)[0] as DisplayAttribute).Name;//获取列字段中文名
                if (!displayName.Equals(string.Empty))
                {
                    try
                    {
                        header.CreateCell(piIndex).SetCellValue(displayName);
                    }
                    catch
                    {
                        header.CreateCell(piIndex).SetCellValue("");
                    }
                }
                piIndex++;
            }

            titleRow.GetCell(0).SetCellValue(title);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, pisLen - 1));//合并单元格，参数为（起始行，结束行，起始列，结束列）
            filterRow.GetCell(0).SetCellValue("筛选条件:" + filter);

            ICellStyle style1 = workbook.CreateCellStyle();
            IFont font1 = workbook.CreateFont();
            font1.Boldweight = short.MaxValue;
            font1.FontHeight = 15 * 20;
            style1.SetFont(font1);
            titleRow.GetCell(0).CellStyle = style1;

            ICellStyle style2 = workbook.CreateCellStyle();
            IFont font2 = workbook.CreateFont();
            font2.Boldweight = short.MaxValue;
            style2.SetFont(font2);
            filterRow.GetCell(0).CellStyle = style2;

            foreach (StatisticVM data in records)
            {
                piIndex = 0;
                IRow dataRow = sheet.CreateRow(rowIndex);
                while (piIndex < pisLen)
                {
                    pi = pis[piIndex];
                    try
                    {
                        if (piIndex >= 1 && piIndex <= 3)
                            dataRow.CreateCell(piIndex).SetCellValue(Convert.ToDouble(pi.GetValue(data, null).ToString()));
                        else
                            dataRow.CreateCell(piIndex).SetCellValue(pi.GetValue(data, null).ToString());
                    }
                    catch
                    {
                        dataRow.CreateCell(piIndex).SetCellValue("");
                    }
                    piIndex++;
                }
                rowIndex++;
            }

            IRow statisticsRow = sheet.CreateRow(rowIndex);
            statisticsRow.CreateCell(0).SetCellValue("合计张数");
            statisticsRow.CreateCell(2).SetCellValue("合计收入");
            statisticsRow.CreateCell(1).SetCellFormula("sum(B4:B" + rowIndex + ")");
            statisticsRow.CreateCell(3).SetCellFormula("sum(D4:D" + rowIndex + ")");

            workbook.Write(ms);

            var fileName = DateTime.Now.ToString("yyyyMMdd") + "交易完成统计报表.xls";

            var temp = ms.GetBuffer();

            ms.Close();
            ms.Dispose();

            return File(temp, "application/vnd.ms-excel", fileName);
        }


        #region 过捡门票详情（交易完成详细）
        [PermissionCheck("view", "DailyReport")]
        public ActionResult DailyReport()
        {
            return View();
        }

        /// <summary>
        /// 过检门票日报表数据
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [PermissionCheck("view", "DailyReport")]
        [HttpPost]
        public ActionResult DailyReportData(DateTime? usedFromTime, DateTime? usedEndTime, int page = 1, int rows = 20)
        {
            var records = rsv.DailyReportData<DailyReportVM>(usedFromTime, usedEndTime);
            var pageList = records.Skip((page - 1) * rows).Take(rows);

            var list = (new int[] { 1 }).Select(x => new { UsedTime = "合计", CurPrice = records.Sum(a => (decimal)a.CurPrice) }).ToList();

            return Json(new { total = records.Count(), rows = pageList, footer = list });
        }

        /// <summary>
        /// 导出过检门票日报表
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [PermissionCheck("export", "DailyReport")]
        public FileResult ExportDailyReportData(DateTime? fromTime, DateTime? endTime)
        {
            var records = rsv.DailyReportData<DailyReportVM>(fromTime, endTime);
            var title = "南顺鳄鱼园 交易完成票详细报表";
            var filter = fromTime.ToString() + "到" + endTime.ToString();
            var memoryStream = RenderToExcel(records, title, filter);

            var fileName = DateTime.Now.ToString("yyyyMMdd") + "交易完成票详细报表.xls";

            var temp = memoryStream.GetBuffer();

            memoryStream.Close();
            memoryStream.Dispose();


            return File(temp, "application/vnd.ms-excel", fileName);
        }

        #endregion

        #region 退票详细统计

        [PermissionCheck("view", "RefundedTickets")]
        public ActionResult RefundedTickets()
        {
            return View();
        }

        [PermissionCheck("view", "RefundedTickets")]
        public ActionResult RefundedTicketsData(DateTime? fromTime, DateTime? endTime)
        {
            var records = rsv.RefundedTicketsData<RefundedTicketVM>(fromTime, endTime);
            var list = (new int[] { 1 }).Select(x => new { BuyTime = "退款额统计", RefundPrice = records.Sum(a => (decimal)a.RefundPrice) }).ToList();

            return Json(new { total = records.Count, rows = records, footer = list});
        }

        [PermissionCheck("export", "RefundedTickets")]
        public FileResult ExportRefundedTicketsData(DateTime? fromTime, DateTime? endTime)
        {
            var records = rsv.RefundedTicketsData<RefundedTicketVM>(fromTime, endTime);
            var title = "南顺鳄鱼园 退票详情统计";
            var filter = fromTime.ToString() + "到" + endTime.ToString();
            var memoryStream = RenderToExcel(records, title, filter);

            var fileName = "南顺鳄鱼园退票详情统计.xls";
            var temp = memoryStream.GetBuffer();
            memoryStream.Close();
            memoryStream.Dispose();

            return File(temp, "application/vnd.ms-excel", fileName);
        }
        #endregion
    }
}
