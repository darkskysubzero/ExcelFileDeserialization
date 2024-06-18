using ExcelFileUpload.API.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelFileUpload.Models {
    public class PositionGridModel {
        public int Id { get; set; }
        [Required] public string OrgNumber { get; set; }
        public string Client { get; set; }
        public string Site { get; set; }
        public string Building { get; set; }
        public string Area { get; set; }
        public string ProcessName { get; set; }
        public string PositionTitle { get; set; }
        public string PositionDescription { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public string EmployeeDisplayName { get; set; }
        public string EmployeeIDNumber { get; set; }
        public string BaseShift { get; set; }
        public string NewBaseShift { get; set; }
        public string ShiftStartEndTime { get; set; }
        public string ShiftType { get; set; }
        public string SalaryWage { get; set; }
        public string ReportsToPosition { get; set; }
        public string TAManager { get; set; }
        public string PositionType { get; set; }
        public DateTime PositionActiveDate { get; set; }
        public string PoolManager { get; set; }
        public string TopSiteManager { get; set; }
        public string PrismaUser { get; set; }
        public string EmailToCreatePrismaUsers { get; set; }
        public string ConfirmAttendance { get; set; }
        public string WorkOnOrgchart { get; set; }
        public string CompleteParticipateWorkflow { get; set; }
        public string MHERequest { get; set; }

        public static Func<Position, PositionGridModel> PositionFunc = (position) => new PositionGridModel {
            Id = position.Id,
            OrgNumber = position.OrgNumber,
            Client = position.Client,
            Site = position.Site,
            Building = position.Building,
            Area = position.Area,
            ProcessName = position.ProcessName,
            PositionTitle = position.PositionTitle,
            PositionDescription = position.PositionDescription,
            EmployeeNumber = position.EmployeeNumber,
            EmployeeName = position.EmployeeName,
            EmployeeSurname = position.EmployeeSurname,
            EmployeeDisplayName = position.EmployeeDisplayName,
            EmployeeIDNumber = position.EmployeeIDNumber,
            BaseShift = position.BaseShift,
            NewBaseShift = position.NewBaseShift,
            ShiftStartEndTime = position.ShiftStartEndTime,
            ShiftType = position.ShiftType,
            SalaryWage = position.SalaryWage,
            ReportsToPosition = position.ReportsToPosition,
            TAManager = position.TAManager,
            PositionType = position.PositionType,
            PositionActiveDate  =position.PositionActiveDate,
            PoolManager = position.PoolManager,
            TopSiteManager = position.TopSiteManager,
            PrismaUser = position.PrismaUser,
            EmailToCreatePrismaUsers = position.EmailToCreatePrismaUsers,
            ConfirmAttendance = position.ConfirmAttendance,
            WorkOnOrgchart = position.WorkOnOrgchart,
            CompleteParticipateWorkflow = position.CompleteParticipateWorkflow,
            MHERequest = position.MHERequest
        };
    }
}
