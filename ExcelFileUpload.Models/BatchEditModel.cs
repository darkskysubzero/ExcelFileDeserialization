using ExcelFileUpload.API.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelFileUpload.Models {
    public class BatchEditModel : PositionGridModel {
        public bool IsDeleted { get; set; }
        public bool IsChanged { get; set; }
        public bool IsNew { get; set; } 
        public List<string> DirtyFields { get; set; }
        
        public BatchEditModel(PositionGridModel position) {
            Id = position.Id;
            OrgNumber = position.OrgNumber;
            Client = position.Client;
            Site = position.Site;
            Building = position.Building;
            Area = position.Area;
            ProcessName = position.ProcessName;
            PositionTitle = position.PositionTitle;
            PositionDescription = position.PositionDescription;
            EmployeeNumber = position.EmployeeNumber;
            EmployeeName = position.EmployeeName;
            EmployeeSurname = position.EmployeeSurname;
            EmployeeDisplayName = position.EmployeeDisplayName;
            EmployeeIDNumber = position.EmployeeIDNumber;
            BaseShift = position.BaseShift;
            NewBaseShift = position.NewBaseShift;
            ShiftStartEndTime = position.ShiftStartEndTime;
            ShiftType = position.ShiftType;
            SalaryWage = position.SalaryWage;
            ReportsToPosition = position.ReportsToPosition;
            TAManager = position.TAManager;
            PositionType = position.PositionType;
            PositionActiveDate = position.PositionActiveDate;
            PoolManager = position.PoolManager;
            TopSiteManager = position.TopSiteManager;
            PrismaUser = position.PrismaUser;
            EmailToCreatePrismaUsers = position.EmailToCreatePrismaUsers;
            ConfirmAttendance = position.ConfirmAttendance;
            WorkOnOrgchart = position.WorkOnOrgchart;
            CompleteParticipateWorkflow = position.CompleteParticipateWorkflow;
            MHERequest = position.MHERequest;
        }

        public BatchEditModel() {
            DirtyFields = new List<string>();
        }
    }
}
