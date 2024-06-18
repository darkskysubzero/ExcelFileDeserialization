namespace ExcelFileUpload.API.Models.Data {
    public class Position {
        public int Id { get; set; }
        public string OrgNumber { get; set; }
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
        public string TAManager { get ; set; }
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
    }
}
