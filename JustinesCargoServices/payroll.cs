using DinkToPdf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace JustinesCargoServices
{
    public partial class payroll : Form
    {
        private bool isGridVisible = false;
        private Timer slideTimer;
        private int targetTop;
        MySqlConnection con = new MySqlConnection(
          "datasource=localhost;" +
          "port=3306;" +
          "database=jcsdb;" +
          "username=root;" +
          "password='';");
        MySqlCommand cmd;
        MySqlDataReader rdr;
        public payroll()
        {
           
            InitializeComponent();
            SlideTimer = new Timer();
            SlideTimer.Interval = 10; // Adjust interval for the animation speed (10ms)
            SlideTimer.Tick += SlideTimer_Tick; // Ad
            SlideTimer1 = new Timer();
            SlideTimer1.Interval = 10; // Adjust interval for the animation speed (10ms)
            SlideTimer1.Tick += SlideTimer1_Tick; // Ad
        }

        private void payroll_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
            txtPassword2.UseSystemPasswordChar = true;
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateHalfDayDeductions();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            payrollPercentageLoad();
            payrollPercentageLoad2();
            hidecoloumns();
            PopulateEmployeeIDs();
        }
        private void LoadPayrollRecords(int employeeID)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;"))
                {
                    con.Open();

                    int currentYear = DateTime.Now.Year;
                    decimal totalNetPay = 0;

                    // SQL query to get payroll records for the selected employee and current year
                    string query = @"
                SELECT Id, EmployeeID, empFname, empLname, RateWage, PerTripWage, OTrateWage, sssD, pagibigD, philhealthD,
                       netPay, grossPay, TotalDeduction, payDate
                FROM payrollrecords
                WHERE EmployeeID = @EmployeeID AND YEAR(payDate) = @CurrentYear";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        cmd.Parameters.AddWithValue("@CurrentYear", currentYear);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dataPayrollRecords.Rows.Clear(); // Clear previous rows

                            while (rdr.Read())
                            {
                                decimal netPay = rdr.GetDecimal(10); // Get netPay column value
                                totalNetPay += netPay; // Sum netPay values

                                dataPayrollRecords.Rows.Add(
                                    rdr.GetInt32(0),  // Id
                                    rdr.GetInt32(1),  // EmployeeID
                                    rdr.GetString(2), // empFname
                                    rdr.GetString(3), // empLname
                                    rdr.GetDecimal(4), // RateWage
                                    rdr.GetDecimal(5), // PerTripWage
                                    rdr.GetDecimal(6), // OTrateWage
                                    rdr.GetDecimal(7), // sssD
                                    rdr.GetDecimal(8), // pagibigD
                                    rdr.GetDecimal(9), // philhealthD
                                    netPay,            // netPay
                                    rdr.GetDecimal(11), // grossPay
                                    rdr.GetDecimal(12), // TotalDeduction
                                    rdr.IsDBNull(13) ? (object)DBNull.Value : rdr.GetDateTime(13).ToString("yyyy-MM-dd") // payDate
                                );
                            }
                        }
                    }

                    // Calculate and display 13th-month pay
                    decimal thirteenthMonthPay = totalNetPay / 12;
                    lbl13thMonthPay.Text = $"13th Month Pay: ₱{thirteenthMonthPay:N2}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateEmployeeIDs()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;"))
                {
                    con.Open();
                    string query = "SELECT EmployeeID FROM empprofiling ORDER BY EmployeeID ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        SortemployeeID.Items.Clear();
                        while (reader.Read())
                        {
                            int employeeID = reader.GetInt32("EmployeeID");
                            SortemployeeID.Items.Add(employeeID); // Add EmployeeID to the ComboBox
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee IDs: " + ex.Message);
            }
        }

        private void hidecoloumns()
        {
            dataTripPay.Columns["cashadvance"].Visible = false;
            dataTripPay.Columns["VAT"].Visible = false;
            dataTripPay.Columns["CHARGES"].Visible = false;
            dataDailyPay.Columns["CASHADVANCE1"].Visible = false;
            dataDailyPay.Columns["VAT1"].Visible = false;
            dataDailyPay.Columns["CHARGES1"].Visible = false;

        }
        private void payrollPercentageLoad()
        {
            try
            {
                // Clear existing rows
                dataratePecentage.Rows.Clear();

                // Define the connection string and use it in a using block
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;Convert Zero Datetime=True;"))
                {
                    con.Open();

                    // SQL query to retrieve data
                    string query = "SELECT ID, from_, to_, sssPercentage, philHealthrate FROM ratepercentage;";

                    // Create and execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Read and populate the DataGridView
                        while (rdr.Read())
                        {
                            dataratePecentage.Rows.Add(
                                rdr.GetInt32(0),   // ID
                                rdr.GetDecimal(1), // from_
                                rdr.GetDecimal(2), // to_
                                rdr.GetDecimal(3), // sssPercentage
                                rdr.GetDecimal(4)  // philHealthrate
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void payrollPercentageLoad2()
        {
            try
            {
                // Clear existing rows
                dataPercentagerate2.Rows.Clear();

                // Define the connection string and use it in a using block
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;Convert Zero Datetime=True;"))
                {
                    con.Open();

                    // SQL query to retrieve data
                    string query = "SELECT ID, from_, to_, sssPercentage, philHealthrate FROM ratepercentage;";

                    // Create and execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Read and populate the DataGridView
                        while (rdr.Read())
                        {
                            dataPercentagerate2.Rows.Add(
                                rdr.GetInt32(0),   // ID
                                rdr.GetDecimal(1), // from_
                                rdr.GetDecimal(2), // to_
                                rdr.GetDecimal(3), // sssPercentage
                                rdr.GetDecimal(4)  // philHealthrate
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void CountEmployeeDays()
        {
            try
            {
                con.Open();

                // Get the holiday dates
                var (regularHolidays, specialHolidays) = GetHolidayDates();
                var allHolidays = regularHolidays.Concat(specialHolidays).ToList();

                // Format the holiday dates for SQL comparison
                var holidayDates = string.Join(",", allHolidays.Select(h => $"'{h.ToString("yyyy-MM-dd")}'"));

                // Count only days that are not holidays, using DATE(TimeIn) to extract the date part
                string countQuery = $@"
            SELECT EmpID, COUNT(*) AS DayCount 
            FROM attendance 
            WHERE DATE(TimeIn) NOT IN ({holidayDates}) 
            GROUP BY EmpID";

                MySqlCommand countCmd = new MySqlCommand(countQuery, con);
                MySqlDataReader countRdr = countCmd.ExecuteReader();

                DataTable attendanceCounts = new DataTable();
                attendanceCounts.Load(countRdr);
                countRdr.Close();

                foreach (DataRow row in attendanceCounts.Rows)
                {
                    string empID = row["EmpID"].ToString();
                    int dayCount = Convert.ToInt32(row["DayCount"]);

                    string updateQuery = @"
                UPDATE empprofiling 
                SET noOfDays = @DayCount 
                WHERE EmployeeID = @EmpID";

                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@EmpID", empID);
                    updateCmd.Parameters.AddWithValue("@DayCount", dayCount);

                    updateCmd.ExecuteNonQuery();
                }

                // Check for employees with no attendance days this month and set noOfDays to 0.00
                string resetQuery = @"
            UPDATE empprofiling 
            SET noOfDays = 0.00 
            WHERE EmployeeID NOT IN (SELECT DISTINCT EmpID FROM attendance WHERE DATE(TimeIn) NOT IN (" + holidayDates + @"))";

                MySqlCommand resetCmd = new MySqlCommand(resetQuery, con);
                resetCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        private void countTrips()
        {
            try
            {
                con.Open();


                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;


                string countTripsQuery = @"
SELECT Driver, COUNT(*) AS TripCount 
FROM delivery_multi 
WHERE MONTH(ArrivalDate) = @CurrentMonth 
AND YEAR(ArrivalDate) = @CurrentYear 
GROUP BY Driver;";

                Dictionary<string, int> driverTrips = new Dictionary<string, int>();

                using (MySqlCommand cmd = new MySqlCommand(countTripsQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);
                    cmd.Parameters.AddWithValue("@CurrentYear", currentYear);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string driver = rdr.GetString("Driver");
                            int tripCount = rdr.GetInt32("TripCount");


                            string[] nameParts = driver.Split(',');
                            if (nameParts.Length > 0)
                            {
                                string lastName = nameParts[0].Trim();


                                driverTrips[lastName] = tripCount;
                            }
                        }
                    }
                }


                foreach (var driver in driverTrips)
                {
                    string updateTripsQuery = "UPDATE empprofiling SET noOfTrips = @noOfTrips WHERE empLname = @empLname;";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateTripsQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@noOfTrips", driver.Value);
                        updateCmd.Parameters.AddWithValue("@empLname", driver.Key);

                        updateCmd.ExecuteNonQuery();
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                con.Close();
                payrollinfoLoad();
            }
        }
        private void CalculateAndUpdateOvertimeForCurrentMonth()
        {
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;" +
                "Convert Zero Datetime=True;"))
            {
                con.Open();

                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // Reset Overtime for current month
                string resetOvertimeQuery = @"
        UPDATE empprofiling 
        SET OThours = 0 
        WHERE EmployeeID IN (SELECT DISTINCT EmpID FROM attendance WHERE MONTH(TimeIn) = @CurrentMonth AND YEAR(TimeIn) = @CurrentYear)";

                using (MySqlCommand resetCmd = new MySqlCommand(resetOvertimeQuery, con))
                {
                    resetCmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);
                    resetCmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                    resetCmd.ExecuteNonQuery();
                }

                // Select attendance records for current month and year
                string selectAttendanceQuery = @"
        SELECT EmpID, TimeIn, Timeout 
        FROM attendance 
        WHERE MONTH(TimeIn) = @CurrentMonth AND YEAR(TimeIn) = @CurrentYear";

                var employeeOvertimes = new Dictionary<int, int>();

                using (MySqlCommand cmd = new MySqlCommand(selectAttendanceQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);
                    cmd.Parameters.AddWithValue("@CurrentYear", currentYear);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int empId = reader.GetInt32("EmpID");

                            // Check for NULL values in TimeIn and Timeout
                            if (!reader.IsDBNull(reader.GetOrdinal("TimeIn")) &&
                                !reader.IsDBNull(reader.GetOrdinal("Timeout")))
                            {
                                DateTime timeIn = reader.GetDateTime("TimeIn");
                                DateTime timeOut = reader.GetDateTime("Timeout");

                                TimeSpan workDuration = timeOut - timeIn;
                                decimal totalHoursWorked = (decimal)workDuration.TotalHours;

                                int overtimeHours = 0;
                                if (totalHoursWorked > 8)
                                {
                                    overtimeHours = (int)Math.Floor(totalHoursWorked - 8);
                                }

                                if (overtimeHours >= 1)
                                {
                                    if (!employeeOvertimes.ContainsKey(empId))
                                    {
                                        employeeOvertimes[empId] = 0;
                                    }
                                    employeeOvertimes[empId] += overtimeHours;
                                }
                            }
                        }
                    }
                }

                // Update the overtime hours in the database
                foreach (var overtime in employeeOvertimes)
                {
                    string updateOvertimeQuery = "UPDATE empprofiling SET OThours = @OThours WHERE EmployeeID = @EmployeeID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateOvertimeQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@OThours", overtime.Value);
                        updateCmd.Parameters.AddWithValue("@EmployeeID", overtime.Key);

                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void CalculateAndUpdateAbsencesForCurrentMonth()
        {
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;" +
                "Convert Zero Datetime=True;"))
            {
                con.Open();

                int currentDay = DateTime.Now.Day;
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                string resetAbsentQuery = @"
            UPDATE empprofiling 
            SET Absent = 0 
            WHERE EmployeeID IN (SELECT DISTINCT EmpID FROM attendance WHERE MONTH(TimeIn) = @CurrentMonth AND YEAR(TimeIn) = @CurrentYear)";

                using (MySqlCommand resetCmd = new MySqlCommand(resetAbsentQuery, con))
                {
                    resetCmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);
                    resetCmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                    resetCmd.ExecuteNonQuery();
                }

                string selectEmployeesQuery = @"
            SELECT EmployeeID 
            FROM empprofiling";

                List<int> allEmployees = new List<int>();

                using (MySqlCommand cmd = new MySqlCommand(selectEmployeesQuery, con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int empId = reader.GetInt32("EmployeeID");
                            allEmployees.Add(empId);
                        }
                    }
                }


                foreach (int empId in allEmployees)
                {
                    int absentDays = 0;


                    for (int day = 1; day <= currentDay; day++)
                    {
                        DateTime dateToCheck = new DateTime(currentYear, currentMonth, day);


                        string checkAttendanceQuery = @"
                    SELECT EmpID 
                    FROM attendance 
                    WHERE EmpID = @EmpID 
                    AND DATE(TimeIn) = @DateToCheck";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkAttendanceQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@EmpID", empId);
                            checkCmd.Parameters.AddWithValue("@DateToCheck", dateToCheck);

                            var result = checkCmd.ExecuteScalar();


                            if (result == null)
                            {
                                absentDays++;
                            }
                        }
                    }


                    string updateAbsentQuery = "UPDATE empprofiling SET Absent = @Absent WHERE EmployeeID = @EmployeeID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateAbsentQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@Absent", absentDays);
                        updateCmd.Parameters.AddWithValue("@EmployeeID", empId);

                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private void CalculateHolidayPay()
        {
            var holidayDates = GetHolidayDates();
            string attendanceQuery = "SELECT EmpID, TimeIn FROM attendance";

            DateTime today = DateTime.Now;

            // Get regular holidays up to the current day in the current month
            var currentMonthRegularHolidays = holidayDates.RegularHolidays
                .Where(date => date.Month == today.Month && date.Year == today.Year && date <= today)
                .ToList();

            // Get special holidays up to the current day in the current month
            var currentMonthSpecialHolidays = holidayDates.SpecialHolidays
                .Where(date => date.Month == today.Month && date.Year == today.Year && date <= today)
                .ToList();


            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;"))
            {
                con.Open();

                // Dictionary to track total holiday pay for each employee
                Dictionary<int, (decimal regularHolidayPay, decimal specialHolidayPay)> holidayPayTotals = new Dictionary<int, (decimal, decimal)>();

                // Step 1: Get all employee IDs
                var employeeIds = GetAllEmployeeIds(con);

                // Step 2: Track which employees have clocked in on holidays
                HashSet<int> employeesTimedInOnHolidays = new HashSet<int>();

                using (MySqlCommand attendanceCmd = new MySqlCommand(attendanceQuery, con))
                using (MySqlDataReader attendanceReader = attendanceCmd.ExecuteReader())
                {
                    while (attendanceReader.Read())
                    {
                        int empId = attendanceReader.GetInt32("EmpID");
                        DateTime timeIn = attendanceReader.GetDateTime("TimeIn");

                        // Fetch Employee RatePerDay from empprofiling table
                        decimal ratePerDay = GetRatePerDay(empId);
                        decimal regularHolidayPay = 0;
                        decimal specialHolidayPay = 0;

                        // Check if TimeIn falls on a holiday in the current month
                        if (currentMonthRegularHolidays.Contains(timeIn.Date))
                        {
                            regularHolidayPay = ratePerDay * 2;  // 200% for regular holiday if worked
                            employeesTimedInOnHolidays.Add(empId);
                        }
                        else if (currentMonthSpecialHolidays.Contains(timeIn.Date))
                        {
                            specialHolidayPay = ratePerDay * 1.3m;  // 130% for special holiday if worked
                            employeesTimedInOnHolidays.Add(empId);
                        }

                        // Accumulate the holiday pay in the dictionary
                        if (holidayPayTotals.ContainsKey(empId))
                        {
                            holidayPayTotals[empId] = (
                                holidayPayTotals[empId].regularHolidayPay + regularHolidayPay,
                                holidayPayTotals[empId].specialHolidayPay + specialHolidayPay
                            );
                        }
                        else
                        {
                            holidayPayTotals[empId] = (regularHolidayPay, specialHolidayPay);
                        }
                    }
                }

                // Step 3: Calculate holiday pay for employees who did not time in on holidays
                foreach (var empId in employeeIds)
                {
                    if (!employeesTimedInOnHolidays.Contains(empId))
                    {
                        // Fetch Employee RatePerDay from empprofiling table
                        decimal ratePerDay = GetRatePerDay(empId);
                        decimal regularHolidayPay = 0;
                        decimal specialHolidayPay = 0;

                        // Calculate 100% pay for holidays not worked
                        foreach (var holidayDate in currentMonthRegularHolidays)
                        {
                            regularHolidayPay = ratePerDay * 1;// 100% of rate per day for regular holidays not worked
                        }

                        foreach (var holidayDate in currentMonthSpecialHolidays)
                        {
                            specialHolidayPay = ratePerDay * 1;// 100% of rate per day for special holidays not worked
                        }

                        // Accumulate or initialize the holiday pay in the dictionary
                        if (holidayPayTotals.ContainsKey(empId))
                        {
                            holidayPayTotals[empId] = (
                                holidayPayTotals[empId].regularHolidayPay + regularHolidayPay,
                                holidayPayTotals[empId].specialHolidayPay + specialHolidayPay
                            );
                        }
                        else
                        {
                            holidayPayTotals[empId] = (regularHolidayPay, specialHolidayPay);
                        }
                    }
                }

                // Step 4: Update the empprofiling table with the calculated holiday pay
                foreach (var empHolidayPay in holidayPayTotals)
                {
                    int empId = empHolidayPay.Key;
                    decimal totalRegularHolidayPay = empHolidayPay.Value.regularHolidayPay;
                    decimal totalSpecialHolidayPay = empHolidayPay.Value.specialHolidayPay;

                    // Update the empprofiling table with the total holiday pay
                    UpdateHolidayPayInProfiling(empId, totalRegularHolidayPay, totalSpecialHolidayPay);
                }
            }
        }

        private List<int> GetAllEmployeeIds(MySqlConnection con)
        {
            List<int> employeeIds = new List<int>();
            string query = "SELECT EmployeeID FROM empprofiling";

            using (MySqlCommand cmd = new MySqlCommand(query, con))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    employeeIds.Add(reader.GetInt32("EmployeeID"));
                }
            }

            return employeeIds;
        }

        private void UpdateHolidayPayInProfiling(int empId, decimal totalRegularHolidayPay, decimal totalSpecialHolidayPay)
        {
            string updateQuery = "UPDATE empprofiling SET RegularHoliday = @RegularHoliday, SpecialHoliday = @SpecialHoliday WHERE EmployeeID = @EmpID";
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;"))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EmpID", empId);
                    cmd.Parameters.AddWithValue("@RegularHoliday", totalRegularHolidayPay);
                    cmd.Parameters.AddWithValue("@SpecialHoliday", totalSpecialHolidayPay);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private decimal GetRatePerDay(int empId)
        {
            // Method to get the rate per day for the employee
            string query = "SELECT RatePerDay FROM empprofiling WHERE EmployeeID = @EmpID";
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;"))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmpID", empId);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        private (List<DateTime> RegularHolidays, List<DateTime> SpecialHolidays) GetHolidayDates()
        {
            // Regular holidays
            List<DateTime> regularHolidays = new List<DateTime>
    {
        new DateTime(DateTime.Now.Year, 12, 1),   // New Year's Day - January 1
        new DateTime(DateTime.Now.Year, 11, 3),   // Araw ng Kagitingan - April 9
        new DateTime(DateTime.Now.Year, 5, 1),   // Labor Day - May 1
        new DateTime(DateTime.Now.Year, 6, 12),  // Independence Day - June 12
        new DateTime(DateTime.Now.Year, 11, 30), // Bonifacio Day - November 30
        new DateTime(DateTime.Now.Year, 12, 25), // Christmas Day - December 25
        new DateTime(DateTime.Now.Year, 12, 30), // Rizal Day - December 30
    };

            // Calculate Holy Week dates
            DateTime easter = CalculateEaster(DateTime.Now.Year);
            DateTime maundyThursday = easter.AddDays(-3); // Maundy Thursday
            DateTime goodFriday = easter.AddDays(-2);     // Good Friday

            regularHolidays.Add(maundyThursday);
            regularHolidays.Add(goodFriday);

            // Special holidays
            List<DateTime> specialHolidays = new List<DateTime>
    {
        new DateTime(DateTime.Now.Year, 2, 25),   // EDSA People Power Anniversary - February 25
        new DateTime(DateTime.Now.Year, 8, 21),   // Ninoy Aquino Day - August 21
        new DateTime(DateTime.Now.Year, 11, 7),   // All Saints' Day - November 1
        new DateTime(DateTime.Now.Year, 12, 8),   // Feast of the Immaculate Conception - December 8
        new DateTime(DateTime.Now.Year, 12, 24),  // Christmas Eve - December 24
        new DateTime(DateTime.Now.Year, 12, 31)   // New Year's Eve - December 31
    };

            DateTime blackSaturday = easter.AddDays(-1); // Black Saturday
            specialHolidays.Add(blackSaturday);

            return (regularHolidays, specialHolidays);
        }

        private DateTime CalculateEaster(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 16;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = (h + l - 7 * m + 114) % 31 + 1;

            return new DateTime(year, month, day);
        }
        public void UpdateHalfDayDeductions()
        {
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;" +
                "Convert Zero Datetime=True;"))
            {
                try
                {
                    con.Open();

                    // Store attendance data in memory
                    var attendanceRecords = new List<(int EmpID, DateTime TimeIn, DateTime TimeOut)>();

                    string attendanceQuery = "SELECT EmpID, TimeIn, TimeOut FROM attendance WHERE TimeIn IS NOT NULL AND TimeOut IS NOT NULL";

                    using (MySqlCommand cmd = new MySqlCommand(attendanceQuery, con))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int empId = reader.GetInt32("EmpID");
                            DateTime timeIn = reader.GetDateTime("TimeIn");
                            DateTime timeOut = reader.GetDateTime("TimeOut");

                            attendanceRecords.Add((empId, timeIn, timeOut));
                        }
                    }

                    // Process each record
                    foreach (var record in attendanceRecords)
                    {
                        double hoursWorked = (record.TimeOut - record.TimeIn).TotalHours;

                        // Check if it's a half-day
                        if (hoursWorked >= 4 && hoursWorked < 8)
                        {
                            UpdateEmployeeHalfDay(record.EmpID, con); // Call the function to update halfDay
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void UpdateEmployeeHalfDay(int empId, MySqlConnection con)
        {
            try
            {
                // Query to fetch RatePerDay
                string selectQuery = "SELECT RatePerDay FROM empprofiling WHERE EmployeeID = @EmpID";

                decimal ratePerDay = 0;

                using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, con))
                {
                    selectCmd.Parameters.AddWithValue("@EmpID", empId);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ratePerDay = reader.GetDecimal("RatePerDay");
                        }
                    }
                }

                // Calculate half-day deduction
                decimal halfDayDeduction = ratePerDay * 0.5m;

                // Update the halfDay column
                string updateQuery = "UPDATE empprofiling SET halfDay = @HalfDay WHERE EmployeeID = @EmpID";

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, con))
                {
                    updateCmd.Parameters.AddWithValue("@HalfDay", halfDayDeduction);
                    updateCmd.Parameters.AddWithValue("@EmpID", empId);
                    updateCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating half-day: " + ex.Message);
            }
        }


        public void UpdateEmployeePayments()
        {
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;" +
                "Convert Zero Datetime=True;"))
            {
                con.Open();

                string selectQuery = "SELECT EmployeeID, Id, LeavePay, halfDay, noOfDays, RatePerDay, RateWage, noOfTrips, grossPay, " +
                                     "PerTripRate, PerTripWage, OThours, OTrateperHour, OTrateWage, CashAdvance, Vat, Charges, " +
                                     "sssD, sssDrate, pagibigD, philhealthD, philhealthDrate, RegularHoliday, SpecialHoliday " +
                                     "FROM empprofiling";

                var employeePayments = new List<EmployeePayment>();

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var payment = new EmployeePayment
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Id = reader.GetInt32("Id"),
                            LeavePay = reader.GetDecimal("LeavePay"),
                            NoOfDays = reader.GetDecimal("noOfDays"),
                            halfDay = reader.GetDecimal("halfDay"),
                            RatePerDay = reader.GetDecimal("RatePerDay"),
                            NoOfTrips = reader.GetDecimal("noOfTrips"),
                            PerTripRate = reader.GetDecimal("PerTripRate"),
                            OThours = reader.GetDecimal("OThours"),
                            OTrateperHour = reader.GetDecimal("OTrateperHour"),
                            OTrateWage = reader.GetDecimal("OTrateWage"),
                            CashAdvance = reader.GetDecimal("CashAdvance"),
                            grossPay = reader.GetDecimal("grossPay"),
                            Vat = reader.GetDecimal("Vat"),
                            Charges = reader.GetDecimal("Charges"),
                            SSSDRate = reader.GetDecimal("sssDrate"),
                            SSSDeduction = reader.GetDecimal("sssD"),
                            PagibigDeduction = reader.GetDecimal("pagibigD"),
                            PhilhealthDeduction = reader.GetDecimal("philhealthD"),
                            philhealthDrate = reader.GetDecimal("philhealthDrate"),
                            RegularHoliday = reader.GetDecimal("RegularHoliday"),
                            SpecialHoliday = reader.GetDecimal("SpecialHoliday")
                        };
                        employeePayments.Add(payment);
                    }
                }

                foreach (var payment in employeePayments)
                {
                    // Ensure all intermediate calculations are accurate
                    decimal hourlyRate = payment.RatePerDay > 0 ? payment.RatePerDay / 8 : 0;
                    decimal overtimeWage = hourlyRate * payment.OThours * 1.25m;
                    decimal rateWage = payment.NoOfDays * payment.RatePerDay;
                    decimal perTripWage = payment.NoOfTrips * payment.PerTripRate;
                    decimal empleavepay = payment.LeavePay;
                    decimal holidayWage = payment.RegularHoliday + payment.SpecialHoliday;

                    // Compute gross pay
                    decimal grossPay = rateWage + perTripWage + overtimeWage + holidayWage + empleavepay;

                    // Get the SSS rate based on the grossPay range
                    decimal sssRate = GetSSSRate(con, grossPay);
                    decimal sssDeduction = sssRate;

                    // Compute other deductions
                    decimal philhealthDrate = grossPay * payment.philhealthDrate;
                    decimal pagibigDrate = payment.PagibigDeduction;
                    decimal totalDeduction = pagibigDrate + philhealthDrate + payment.CashAdvance + payment.Vat + payment.Charges + sssDeduction;

                    // Compute net pay
                    decimal netPay = grossPay - (totalDeduction + payment.halfDay);

                    // Update the database
                    string updateQuery = "UPDATE empprofiling SET RateWage = @RateWage, pagibigD = @pagibigD, PerTripWage = @PerTripWage, " +
                                         "OTrateperHour = @OTrateperHour, OTrateWage = @OTrateWage, sssD = @sssD, philhealthD = @philhealthD, " +
                                         "TotalDeduction = @TotalDeduction, NetPay = @NetPay, GrossPay = @GrossPay WHERE Id = @Id";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@RateWage", rateWage);
                        updateCmd.Parameters.AddWithValue("@PerTripWage", perTripWage);
                        updateCmd.Parameters.AddWithValue("@OTrateperHour", hourlyRate);
                        updateCmd.Parameters.AddWithValue("@OTrateWage", overtimeWage);
                        updateCmd.Parameters.AddWithValue("@pagibigD", pagibigDrate);
                        updateCmd.Parameters.AddWithValue("@sssD", sssDeduction);
                        updateCmd.Parameters.AddWithValue("@philhealthD", philhealthDrate);
                        updateCmd.Parameters.AddWithValue("@TotalDeduction", totalDeduction);
                        updateCmd.Parameters.AddWithValue("@NetPay", netPay);
                        updateCmd.Parameters.AddWithValue("@GrossPay", grossPay);
                        updateCmd.Parameters.AddWithValue("@Id", payment.Id);

                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private decimal GetSSSRate(MySqlConnection con, decimal grossPay)
        {
            string query = "SELECT sssPercentage FROM ratepercentage WHERE @GrossPay BETWEEN `from_` AND `to_`";
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@GrossPay", grossPay);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }


        private class EmployeePayment
        {
            public int EmployeeID { get; set; }
            public int Id { get; set; }
            public decimal Absent { get; set; }
            public decimal AbsentRate { get; set; }
            public decimal LeavePay { get; set; }
            public decimal grossPay { get; set; }
            public decimal NoOfDays { get; set; }
            public decimal RatePerDay { get; set; }
            public decimal SSSDRate { get; set; }
            public decimal philhealthDrate { get; set; }
            
            public decimal halfDay { get; set; }
            public decimal NoOfTrips { get; set; }
            public decimal PerTripRate { get; set; }
            public decimal OThours { get; set; }
            public decimal OTrateperHour { get; set; }
            public decimal OTrateWage { get; set; }
            public decimal CashAdvance { get; set; }
            public decimal Vat { get; set; }
            public decimal Charges { get; set; }
            public decimal SSSDeduction { get; set; }
            public decimal PagibigDeduction { get; set; }
            public decimal PhilhealthDeduction { get; set; }
            public decimal RegularHoliday { get; set; } // Added for Regular Holiday
            public decimal SpecialHoliday { get; set; } // Added for Special Holiday
        }
        





        private void CalculateAndUpdate13thMonthPayForCurrentYear()
        {
            using (MySqlConnection con = new MySqlConnection(
                "datasource=localhost;" +
                "port=3306;" +
                "database=JCSdb;" +
                "username=root;" +
                "password=;" +
                "Convert Zero Datetime=True;"))
            {
                con.Open();


                int currentYear = DateTime.Now.Year;


                string selectNetPayQuery = @"
            SELECT EmployeeID, SUM(netPay) AS TotalNetPay
            FROM payrollrecords
            WHERE YEAR(payDate) = @CurrentYear
            GROUP BY EmployeeID";

                var employeeNetPays = new List<(int EmployeeID, decimal TotalNetPay)>();

                using (MySqlCommand cmd = new MySqlCommand(selectNetPayQuery, con))
                {
                    cmd.Parameters.AddWithValue("@CurrentYear", currentYear);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int employeeId = reader.GetInt32("EmployeeID");
                            decimal totalNetPay = reader.GetDecimal("TotalNetPay");


                            employeeNetPays.Add((employeeId, totalNetPay));
                        }
                    }
                }


                foreach (var employeeNetPay in employeeNetPays)
                {

                    decimal thirteenthMonthPay = employeeNetPay.TotalNetPay / 12;


                    string updateThirteenthMonthPayQuery = @"
                UPDATE empprofiling
                SET thirteenMonthPay = @ThirteenthMonthPay
                WHERE EmployeeID = @EmployeeID";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateThirteenthMonthPayQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@ThirteenthMonthPay", thirteenthMonthPay);
                        updateCmd.Parameters.AddWithValue("@EmployeeID", employeeNetPay.EmployeeID);

                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private void CountEmployeeDaysforthismonth()
        {
            try
            {
                con.Open();

                // Get the current year and month
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                // Query to count attendance for the current month
                string countQuery = @"
            SELECT EmpID, COUNT(*) AS DayCount 
            FROM attendance 
            WHERE YEAR(TimeIn) = @CurrentYear AND MONTH(TimeIn) = @CurrentMonth
            GROUP BY EmpID";

                MySqlCommand countCmd = new MySqlCommand(countQuery, con);
                // Add parameters for the current year and month
                countCmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                countCmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

                MySqlDataReader countRdr = countCmd.ExecuteReader();

                DataTable attendanceCounts = new DataTable();
                attendanceCounts.Load(countRdr);
                countRdr.Close();

                foreach (DataRow row in attendanceCounts.Rows)
                {
                    string empID = row["EmpID"].ToString();
                    int dayCount = Convert.ToInt32(row["DayCount"]);

                    string updateQuery = @"
                UPDATE empprofiling 
                SET noOfDays = @DayCount 
                WHERE EmployeeID = @EmpID";

                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@EmpID", empID);
                    updateCmd.Parameters.AddWithValue("@DayCount", dayCount);

                    updateCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        private void payrollinfoLoad()
        {
            try
            {
                // Define the connection string and use it in a using block
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;Convert Zero Datetime=True;"))
                {
                    con.Open();

                    // SQL query to get payroll data
                    string query = "SELECT Id, EmployeeID, empFname, empLname, Absent, LeavePay, paymentMethod, noOfDays, RatePerDay, RateWage, " +
                                   "noOfTrips, PerTripRate, PerTripWage, OThours, OTrateperHour, OTrateWage, RegularHoliday, SpecialHoliday, sssD, " +
                                   "pagibigD, philhealthD, netPay, grossPay, TotalDeduction, thirteenMonthPay, dateofPay FROM empprofiling;";

                    // Create and execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Clear previous rows in the DataGridView
                        dataPayrollInfo.Rows.Clear();

                        // Read the data and populate the DataGridView
                        while (rdr.Read())
                        {
                            dataPayrollInfo.Rows.Add(
                                rdr.GetInt32(0),  // Id
                                rdr.GetInt32(1),  // EmployeeID
                                rdr.GetString(2), // empFname
                                rdr.GetString(3), // empLname
                                rdr.GetDecimal(4), // Absent
                                rdr.GetDecimal(5), // LeavePay
                                rdr.GetString(6), // paymentMethod
                                rdr.GetDecimal(7), // noOfDays
                                rdr.GetDecimal(8), // RatePerDay
                                rdr.GetDecimal(9), // RateWage
                                rdr.GetDecimal(10), // noOfTrips
                                rdr.GetDecimal(11), // PerTripRate
                                rdr.GetDecimal(12), // PerTripWage
                                rdr.GetDecimal(13), // OThours
                                rdr.GetDecimal(14), // OTrateperHour
                                rdr.GetDecimal(15), // OTrateWage
                                rdr.GetDecimal(16), // RegularHoliday
                                rdr.GetDecimal(17), // SpecialHoliday
                                rdr.GetDecimal(18), // sssD
                                rdr.GetDecimal(19), // pagibigD
                                rdr.GetDecimal(20), // philhealthD
                                rdr.GetDecimal(21), // netPay
                                rdr.GetDecimal(22), // grossPay
                                rdr.GetDecimal(23), // TotalDeduction
                                rdr.GetDecimal(24), // thirteenMonthPay
                                rdr.IsDBNull(25) ? (object)DBNull.Value : rdr.GetDateTime(25).ToString("yyyy-MM-dd") // dateofPay
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void payrollDailyLoad()
        {
            try
            {
                // Define the connection string and use it in a using block
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;Convert Zero Datetime=True;"))
                {
                    con.Open();

                    // SQL query to filter for "Monthly" or "Semi-Monthly" payment method
                    string query = "SELECT Id, EmployeeID, empFname, empLname, Position, Absent, LeavePay, paymentMethod, noOfDays, RatePerDay, RateWage, OThours, OTrateperHour, OTrateWage," +
                        " RegularHoliday, SpecialHoliday, sssD, pagibigD, philhealthD, netPay, grossPay, TotalDeduction, thirteenMonthPay, dateofPay, CashAdvance,Charges,Vat " +
                                   "FROM empprofiling WHERE paymentMethod IN ('Monthly', 'Semi-Monthly');";

                    // Create and execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Clear the existing rows in the DataGridView
                        dataDailyPay.Rows.Clear();

                        // Read data and populate the DataGridView
                        while (rdr.Read())
                        {
                            dataDailyPay.Rows.Add(
                                rdr.GetInt32(0),  // Id
                                rdr.GetInt32(1),  // EmployeeID
                                rdr.GetString(2), // empFname
                                rdr.GetString(3), // empLname
                                rdr.GetString(4), // Position
                                rdr.GetDecimal(5), // Absent
                                rdr.GetDecimal(6), // LeavePay
                                rdr.GetString(7), // paymentMethod
                                rdr.GetDecimal(8), // noOfDays
                                rdr.GetDecimal(9), // RatePerDay
                                rdr.GetDecimal(10), // RateWage
                                rdr.GetDecimal(11), // OThours
                                rdr.GetDecimal(12), // OTrateperHour
                                rdr.GetDecimal(13), // OTrateWage
                                rdr.GetDecimal(14), // RegularHoliday
                                rdr.GetDecimal(15), // SpecialHoliday
                                rdr.GetDecimal(16), // sssD
                                rdr.GetDecimal(17), // pagibigD
                                rdr.GetDecimal(18), // philhealthD
                                rdr.GetDecimal(19), // netPay
                                rdr.GetDecimal(20), // grossPay
                                rdr.GetDecimal(21), // TotalDeduction
                                rdr.GetDecimal(22), // thirteenMonthPay
                                rdr.IsDBNull(23) ? (object)DBNull.Value : rdr.GetDateTime(23).ToString("yyyy-MM-dd"), // dateofPay
                                rdr.GetDecimal(24), // grossPay
                                rdr.GetDecimal(25), // TotalDeduction
                                rdr.GetDecimal(26) // thirteenMonthPay
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void payrollTripLoad()
        {
            try
            {
                // Define the connection string and use it in a using block
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;Convert Zero Datetime=True;"))
                {
                    con.Open();

                    // SQL query to filter by "Per Trip" under paymentMethod
                    string query = "SELECT Id, EmployeeID, empFname, empLname, Position, Absent, LeavePay, paymentMethod, " +
                                   "noOfTrips, PerTripRate, PerTripWage, OThours, OTrateperHour, OTrateWage, RegularHoliday, " +
                                   "SpecialHoliday, sssD, pagibigD, philhealthD, netPay, grossPay, TotalDeduction, thirteenMonthPay, " +
                                   "dateofPay,CashAdvance,Charges,Vat FROM empprofiling WHERE paymentMethod = 'Per Trip';";

                    // Create and execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Clear the existing rows in the DataGridView
                        dataTripPay.Rows.Clear();

                        // Read data and populate the DataGridView
                        while (rdr.Read())
                        {
                            dataTripPay.Rows.Add(
                                rdr.GetInt32(0),  // Id
                                rdr.GetInt32(1),  // EmployeeID
                                rdr.GetString(2), // empFname
                                rdr.GetString(3), // empLname
                                rdr.GetString(4), // Position
                                rdr.GetDecimal(5), // Absent
                                rdr.GetDecimal(6), // LeavePay
                                rdr.GetString(7), // paymentMethod
                                rdr.GetDecimal(8), // noOfTrips
                                rdr.GetDecimal(9), // PerTripRate
                                rdr.GetDecimal(10), // PerTripWage
                                rdr.GetDecimal(11), // OThours
                                rdr.GetDecimal(12), // OTrateperHour
                                rdr.GetDecimal(13), // OTrateWage
                                rdr.GetDecimal(14), // RegularHoliday
                                rdr.GetDecimal(15), // SpecialHoliday
                                rdr.GetDecimal(16), // sssD
                                rdr.GetDecimal(17), // pagibigD
                                rdr.GetDecimal(18), // philhealthD
                                rdr.GetDecimal(19), // netPay
                                rdr.GetDecimal(20), // grossPay
                                rdr.GetDecimal(21), // TotalDeduction
                                rdr.GetDecimal(22), // thirteenMonthPay
                                rdr.IsDBNull(23) ? (object)DBNull.Value : rdr.GetDateTime(23).ToString("yyyy-MM-dd"), // dateofPay
                                rdr.GetDecimal(24), // grossPay
                                rdr.GetDecimal(25), // TotalDeduction
                                rdr.GetDecimal(26) // thirteenMonthPay
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }



        private void payrollrecordsLoad()
        {
            try
            {
                // Define the connection string and use it in a using block
                using (MySqlConnection con = new MySqlConnection("datasource=localhost;port=3306;database=JCSdb;username=root;password=;Convert Zero Datetime=True;"))
                {
                    con.Open();

                    // SQL query to get payroll records
                    string query = "SELECT Id, EmployeeID, empFname, empLname, RateWage, PerTripWage, OTrateWage, sssD, pagibigD, philhealthD, " +
                                   "netPay, grossPay, TotalDeduction , payDate FROM payrollrecords;";

                    // Create and execute the command
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // Clear previous rows in the DataGridView
                        dataPayrollRecords.Rows.Clear();

                        // Read the data and populate the DataGridView
                        while (rdr.Read())
                        {
                            dataPayrollRecords.Rows.Add(
                                rdr.GetInt32(0),  // Id
                                rdr.GetInt32(1),  // EmployeeID
                                rdr.GetString(2), // empFname
                                rdr.GetString(3), // empLname
                                rdr.GetDecimal(4), // RateWage
                                rdr.GetDecimal(5), // PerTripWage
                                rdr.GetDecimal(6), // OTrateWage
                                rdr.GetDecimal(7), // sssD
                                rdr.GetDecimal(8), // pagibigD
                                rdr.GetDecimal(9), // philhealthD
                                rdr.GetDecimal(10), // netPay
                                rdr.GetDecimal(11), // grossPay
                                rdr.GetDecimal(12), // TotalDeduction
                                rdr.IsDBNull(13) ? (object)DBNull.Value : rdr.GetDateTime(13).ToString("yyyy-MM-dd") // payDate
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void btnCheckPay_Click(object sender, EventArgs e)
        {
            pnlShowPayOp.Visible = !pnlShowPayOp.Visible;
        }

        private void btnShowRecords_Click(object sender, EventArgs e)
        {
            pnlShowPayOp.Visible = !pnlShowPayOp.Visible;

        }

        private void btnPaySlip_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnMontly_Click(object sender, EventArgs e)
        {
            pnlMontlyy.Visible = !pnlMontlyy.Visible;
            pnlShowPayOp.Visible = !pnlShowPayOp.Visible;

        }

        private void btnPerTrip_Click(object sender, EventArgs e)
        {
            pnlPerTrip.Visible = !pnlPerTrip.Visible;
            pnlShowPayOp.Visible = !pnlShowPayOp.Visible;
        }

        private void guna2Panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataDailyPay_SelectionChanged(object sender, EventArgs e)
        {
            if (dataDailyPay.SelectedRows.Count > 0)
            {
                txtID.Text = dataDailyPay.SelectedRows[0].Cells[0].Value.ToString();
                emppID.Text = dataDailyPay.SelectedRows[0].Cells[1].Value.ToString();
                txtFname.Text = dataDailyPay.SelectedRows[0].Cells[2].Value.ToString();
                txtLname.Text = dataDailyPay.SelectedRows[0].Cells[3].Value.ToString();
                txtPosition.Text = dataDailyPay.SelectedRows[0].Cells[4].Value.ToString();
                txtAbsent.Text = dataDailyPay.SelectedRows[0].Cells[5].Value.ToString();
                txtLeavePay.Text = dataDailyPay.SelectedRows[0].Cells[6].Value.ToString();
                txtpaymentMet.Text = dataDailyPay.SelectedRows[0].Cells[7].Value.ToString();
                txtNodays.Text = dataDailyPay.SelectedRows[0].Cells[8].Value.ToString();
                txtratePday.Text = dataDailyPay.SelectedRows[0].Cells[9].Value.ToString();
                txtrateWage.Text = dataDailyPay.SelectedRows[0].Cells[10].Value.ToString();
                txtOThour.Text = dataDailyPay.SelectedRows[0].Cells[11].Value.ToString();
                txtOTrateperHr.Text = dataDailyPay.SelectedRows[0].Cells[12].Value.ToString();
                txtOTwage.Text = dataDailyPay.SelectedRows[0].Cells[13].Value.ToString();
                txtRegHol.Text = dataDailyPay.SelectedRows[0].Cells[14].Value.ToString();
                txtSpecHol.Text = dataDailyPay.SelectedRows[0].Cells[15].Value.ToString();
                txtSSS.Text = dataDailyPay.SelectedRows[0].Cells[16].Value.ToString();
                txtPagibig.Text = dataDailyPay.SelectedRows[0].Cells[17].Value.ToString();
                txtPhilhealth.Text = dataDailyPay.SelectedRows[0].Cells[18].Value.ToString();
                txtNet.Text = dataDailyPay.SelectedRows[0].Cells[19].Value.ToString();
                txtGross.Text = dataDailyPay.SelectedRows[0].Cells[20].Value.ToString();
                txtDeduction.Text = dataDailyPay.SelectedRows[0].Cells[21].Value.ToString();
                txt13thMonth.Text = dataDailyPay.SelectedRows[0].Cells[22].Value.ToString();
                Pickerdateofpay.Value = dataDailyPay.SelectedRows[0].Cells[23].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataDailyPay.SelectedRows[0].Cells[23].Value);
                txtCashAdvance.Text = dataDailyPay.SelectedRows[0].Cells[24].Value.ToString();
                txtCharges6.Text = dataDailyPay.SelectedRows[0].Cells[25].Value.ToString();
                txtVat.Text = dataDailyPay.SelectedRows[0].Cells[26].Value.ToString();

                txtID2.Text = dataDailyPay.SelectedRows[0].Cells[0].Value.ToString();
                emppID2.Text = dataDailyPay.SelectedRows[0].Cells[1].Value.ToString();
                txtFname2.Text = dataDailyPay.SelectedRows[0].Cells[2].Value.ToString();
                txtLname2.Text = dataDailyPay.SelectedRows[0].Cells[3].Value.ToString();
                txtPosition2.Text = dataDailyPay.SelectedRows[0].Cells[4].Value.ToString();
                txtAbsent2.Text = dataDailyPay.SelectedRows[0].Cells[5].Value.ToString();
                txtLeavePay2.Text = dataDailyPay.SelectedRows[0].Cells[6].Value.ToString();
                txtpaymentMet2.Text = dataDailyPay.SelectedRows[0].Cells[7].Value.ToString();
                txtNodays2.Text = dataDailyPay.SelectedRows[0].Cells[8].Value.ToString();
                txtratePday2.Text = dataDailyPay.SelectedRows[0].Cells[9].Value.ToString();
                txtrateWage.Text = dataDailyPay.SelectedRows[0].Cells[10].Value.ToString();
                txtOThour2.Text = dataDailyPay.SelectedRows[0].Cells[11].Value.ToString();
                txtOTrateperHr2.Text = dataDailyPay.SelectedRows[0].Cells[12].Value.ToString();
                txtOTwage.Text = dataDailyPay.SelectedRows[0].Cells[13].Value.ToString();
                txtRegHol2.Text = dataDailyPay.SelectedRows[0].Cells[14].Value.ToString();
                txtSpecHol2.Text = dataDailyPay.SelectedRows[0].Cells[15].Value.ToString();
                txtSSS2.Text = dataDailyPay.SelectedRows[0].Cells[16].Value.ToString();
                txtPagibig2.Text = dataDailyPay.SelectedRows[0].Cells[17].Value.ToString();
                txtPhilhealth2.Text = dataDailyPay.SelectedRows[0].Cells[18].Value.ToString();
                txtNet.Text = dataDailyPay.SelectedRows[0].Cells[19].Value.ToString();
                txtGross.Text = dataDailyPay.SelectedRows[0].Cells[20].Value.ToString();
                txtDeduction.Text = dataDailyPay.SelectedRows[0].Cells[21].Value.ToString();
                txt13thMonth2.Text = dataDailyPay.SelectedRows[0].Cells[22].Value.ToString();
                Pickerdateofpay.Value = dataDailyPay.SelectedRows[0].Cells[23].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataDailyPay.SelectedRows[0].Cells[23].Value);
            
            }
        }

        private void guna2TextBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            con.Open();
            cmd = new MySqlCommand("UPDATE empprofiling SET " +
                                       "empFname=@empFname," +
                                       " empLname=@empLname," +
                                      

                                       " paymentMethod=@paymentMethod, " +
                                       "noOfDays=@noOfDays, " +
                                       "RatePerDay=@RatePerDay, " +
                                       "RateWage=@RateWage, " +
                                     
                                 
                                       "RegularHoliday=@RegularHoliday, " +

                                       "SpecialHoliday=@SpecialHoliday, " +

                                       "CashAdvance=@CashAdvance, " +
                                       "Vat=@Vat, " +
                                       "Charges=@Charges, " +
                                       "OThours=@OThours, " +
                                       "OTrateperHour=@OTrateperHour, " +
                                       "OTrateWage=@OTrateWage, " +
                                       "sssD=@sssD, " +
                                       "pagibigD=@pagibigD, " +
                                       "philhealthD=@philhealthD, " +
                                       "netPay=@netPay, " +
                                       "grossPay=@grossPay, " +
                                       "TotalDeduction=@TotalDeduction " +

                                       "WHERE EmployeeID=@EmployeeID", con);


            cmd.Parameters.AddWithValue("@EmployeeID", emppID.Text);
            cmd.Parameters.AddWithValue("@RegularHoliday", txtRegHol.Text);
            cmd.Parameters.AddWithValue("@SpecialHoliday", txtSpecHol.Text);
            cmd.Parameters.AddWithValue("@CashAdvance", txtCashAdvance.Text);
            cmd.Parameters.AddWithValue("@Vat", txtVat.Text);
            cmd.Parameters.AddWithValue("@Charges", txtCharges6.Text);
            cmd.Parameters.AddWithValue("@empFname", txtFname.Text);
            cmd.Parameters.AddWithValue("@empLname", txtLname.Text);
            cmd.Parameters.AddWithValue("@paymentMethod", txtpaymentMet.Text);
            cmd.Parameters.AddWithValue("@noOfDays", txtNodays.Text);
            cmd.Parameters.AddWithValue("@RatePerDay", txtratePday.Text);
            cmd.Parameters.AddWithValue("@RateWage", txtrateWage.Text);
         
            cmd.Parameters.AddWithValue("@OThours", txtOThour.Text);
            cmd.Parameters.AddWithValue("@OTrateperHour", txtOTrateperHr.Text);
            cmd.Parameters.AddWithValue("@OTrateWage", txtOTwage.Text);
            cmd.Parameters.AddWithValue("@sssD", txtSSS.Text);
            cmd.Parameters.AddWithValue("@pagibigD", txtPagibig.Text);
            cmd.Parameters.AddWithValue("@philhealthD", txtPhilhealth.Text);
            cmd.Parameters.AddWithValue("@netPay", txtNet.Text);
            cmd.Parameters.AddWithValue("@grossPay", txtGross.Text);
            cmd.Parameters.AddWithValue("@TotalDeduction", txtDeduction.Text);

            cmd.ExecuteNonQuery();



            con.Close();
            MessageBox.Show("Record Updated Successfully!", "Success");
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateHalfDayDeductions();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            hidecoloumns();
        }

        private void btnManual_Click(object sender, EventArgs e)
        {
            
            pnlAdminLog.Visible = !pnlAdminLog.Visible;
        }
        private void ToggleTextBoxVisibility()
        {
            txtID.Visible = !txtID.Visible;
            emppID.Visible = !emppID.Visible;
            txtFname.Visible = !txtFname.Visible;
            txtLname.Visible = !txtLname.Visible;
            txtPosition.Visible = !txtPosition.Visible;
            txtAbsent.Visible = !txtAbsent.Visible;
            txtLeavePay.Visible = !txtLeavePay.Visible;
            txtpaymentMet.Visible = !txtpaymentMet.Visible;
            txtNodays.Visible = !txtNodays.Visible;
            txtratePday.Visible = !txtratePday.Visible;
            pnlpercentrate.Visible = !pnlpercentrate.Visible;
            txtOThour.Visible = !txtOThour.Visible;
            txtOTrateperHr.Visible = !txtOTrateperHr.Visible;
     
            txtRegHol.Visible = !txtRegHol.Visible;
            txtSpecHol.Visible = !txtSpecHol.Visible;
            txtSSS.Visible = !txtSSS.Visible;
            txtPagibig.Visible = !txtPagibig.Visible;
            txtPhilhealth.Visible = !txtPhilhealth.Visible;
          
            txt13thMonth.Visible = !txt13thMonth.Visible;

            txtID2.Visible = !txtID2.Visible;
            emppID2.Visible = !emppID2.Visible;
            txtFname2.Visible = !txtFname2.Visible;
            txtLname2.Visible = !txtLname2.Visible;
            txtPosition2.Visible = !txtPosition2.Visible;
            txtAbsent2.Visible = !txtAbsent2.Visible;
            txtLeavePay2.Visible = !txtLeavePay2.Visible;
            txtpaymentMet2.Visible = !txtpaymentMet2.Visible;
            txtNodays2.Visible = !txtNodays2.Visible;
            txtratePday2.Visible = !txtratePday2.Visible;
          
            txtOThour2.Visible = !txtOThour2.Visible;
            txtOTrateperHr2.Visible = !txtOTrateperHr2.Visible;
           
            txtRegHol2.Visible = !txtRegHol2.Visible;
            txtSpecHol2.Visible = !txtSpecHol2.Visible;
            txtSSS2.Visible = !txtSSS2.Visible;
            txtPagibig2.Visible = !txtPagibig2.Visible;
            txtPhilhealth2.Visible = !txtPhilhealth2.Visible;
          
            txt13thMonth2.Visible = !txt13thMonth2.Visible;

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void label72_Click(object sender, EventArgs e)
        {

        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand("UPDATE empprofiling SET " +
                                       "empFname=@empFname," +
                                       " empLname=@empLname," +


                                       " paymentMethod=@paymentMethod, " +
                                       "noOfTrips=@noOfTrips, " +
                                       "PerTripRate=@PerTripRate, " +
                                       "PerTripWage=@PerTripWage, " +


                                       "RegularHoliday=@RegularHoliday, " +

                                       "SpecialHoliday=@SpecialHoliday, " +

                                       "CashAdvance=@CashAdvance, " +
                                       "Vat=@Vat, " +
                                       "Charges=@Charges, " +
                                       "OThours=@OThours, " +
                                       "OTrateperHour=@OTrateperHour, " +
                                       "OTrateWage=@OTrateWage, " +
                                       "sssD=@sssD, " +
                                       "pagibigD=@pagibigD, " +
                                       "philhealthD=@philhealthD, " +
                                       "netPay=@netPay, " +
                                       "grossPay=@grossPay, " +
                                       "TotalDeduction=@TotalDeduction " +

                                       "WHERE EmployeeID=@EmployeeID", con);


            cmd.Parameters.AddWithValue("@EmployeeID", emppID3.Text);
            cmd.Parameters.AddWithValue("@RegularHoliday", txtRegHol3.Text);
            cmd.Parameters.AddWithValue("@SpecialHoliday", txtSpecHol3.Text);
            cmd.Parameters.AddWithValue("@CashAdvance", txtCashAdvance2.Text);
            cmd.Parameters.AddWithValue("@Vat", txtVat2.Text);
            cmd.Parameters.AddWithValue("@Charges", txtCharges2.Text);
            cmd.Parameters.AddWithValue("@empFname", txtFname3.Text);
            cmd.Parameters.AddWithValue("@empLname", txtLname3.Text);
            cmd.Parameters.AddWithValue("@paymentMethod", txtpaymentMet3.Text);
            cmd.Parameters.AddWithValue("@noOfTrips", txtNoTrips3.Text);
            cmd.Parameters.AddWithValue("@PerTripRate", txtRateTrip3.Text);
            cmd.Parameters.AddWithValue("@PerTripWage", txtrateWage3.Text);

            cmd.Parameters.AddWithValue("@OThours", txtOThour3.Text);
            cmd.Parameters.AddWithValue("@OTrateperHour", txtOTrateperHr3.Text);
            cmd.Parameters.AddWithValue("@OTrateWage", txtOTwage3.Text);
            cmd.Parameters.AddWithValue("@sssD", txtSSS3.Text);
            cmd.Parameters.AddWithValue("@pagibigD", txtPagibig3.Text);
            cmd.Parameters.AddWithValue("@philhealthD", txtPhilhealth3.Text);
            cmd.Parameters.AddWithValue("@netPay", txtNet3.Text);
            cmd.Parameters.AddWithValue("@grossPay", txtGross3.Text);
            cmd.Parameters.AddWithValue("@TotalDeduction", txtDeduction3.Text);

            cmd.ExecuteNonQuery();



            con.Close();
            MessageBox.Show("Record Updated Successfully!", "Success");
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateHalfDayDeductions();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            hidecoloumns();
        }

        private void btnManual2_Click(object sender, EventArgs e)
        {
          
            pnlAdminLog2.Visible = !pnlAdminLog2.Visible;
        }

        private void dataTripPay_SelectionChanged(object sender, EventArgs e)
        {
            if (dataTripPay.SelectedRows.Count > 0)
            {
                txtID3.Text = dataTripPay.SelectedRows[0].Cells[0].Value.ToString();
                emppID3.Text = dataTripPay.SelectedRows[0].Cells[1].Value.ToString();
                txtFname3.Text = dataTripPay.SelectedRows[0].Cells[2].Value.ToString();
                txtLname3.Text = dataTripPay.SelectedRows[0].Cells[3].Value.ToString();
                txtPosition3.Text = dataTripPay.SelectedRows[0].Cells[4].Value.ToString();
                txtAbsent3.Text = dataTripPay.SelectedRows[0].Cells[5].Value.ToString();
                txtLeavePay3.Text = dataTripPay.SelectedRows[0].Cells[6].Value.ToString();
                txtpaymentMet3.Text = dataTripPay.SelectedRows[0].Cells[7].Value.ToString();
                txtNoTrips3.Text = dataTripPay.SelectedRows[0].Cells[8].Value.ToString();
                txtRateTrip3.Text = dataTripPay.SelectedRows[0].Cells[9].Value.ToString();
                txtrateWage3.Text = dataTripPay.SelectedRows[0].Cells[10].Value.ToString();
                txtOThour3.Text = dataTripPay.SelectedRows[0].Cells[11].Value.ToString();
                txtOTrateperHr3.Text = dataTripPay.SelectedRows[0].Cells[12].Value.ToString();
                txtOTwage3.Text = dataTripPay.SelectedRows[0].Cells[13].Value.ToString();
                txtRegHol3.Text = dataTripPay.SelectedRows[0].Cells[14].Value.ToString();
                txtSpecHol3.Text = dataTripPay.SelectedRows[0].Cells[15].Value.ToString();
                txtSSS3.Text = dataTripPay.SelectedRows[0].Cells[16].Value.ToString();
                txtPagibig3.Text = dataTripPay.SelectedRows[0].Cells[17].Value.ToString();
                txtPhilhealth3.Text = dataTripPay.SelectedRows[0].Cells[18].Value.ToString();
                txtNet3.Text = dataTripPay.SelectedRows[0].Cells[19].Value.ToString();
                txtGross3.Text = dataTripPay.SelectedRows[0].Cells[20].Value.ToString();
                txtDeduction3.Text = dataTripPay.SelectedRows[0].Cells[21].Value.ToString();
                txt13thMonth3.Text = dataTripPay.SelectedRows[0].Cells[22].Value.ToString();
                Pickerdateofpay3.Value = dataTripPay.SelectedRows[0].Cells[23].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataTripPay.SelectedRows[0].Cells[23].Value);
              

                txtID4.Text = dataTripPay.SelectedRows[0].Cells[0].Value.ToString();
                emppID4.Text = dataTripPay.SelectedRows[0].Cells[1].Value.ToString();
                txtFname4.Text = dataTripPay.SelectedRows[0].Cells[2].Value.ToString();
                txtLname4.Text = dataTripPay.SelectedRows[0].Cells[3].Value.ToString();
                txtPosition4.Text = dataTripPay.SelectedRows[0].Cells[4].Value.ToString();
                txtAbsent4.Text = dataTripPay.SelectedRows[0].Cells[5].Value.ToString();
                txtLeavePay4.Text = dataTripPay.SelectedRows[0].Cells[6].Value.ToString();
                txtpaymentMet4.Text = dataTripPay.SelectedRows[0].Cells[7].Value.ToString();
                txtNoTrips4.Text = dataTripPay.SelectedRows[0].Cells[8].Value.ToString();
                txtRateTrip4.Text = dataTripPay.SelectedRows[0].Cells[9].Value.ToString();
                txtrateWage3.Text = dataTripPay.SelectedRows[0].Cells[10].Value.ToString();
                txtOThour4.Text = dataTripPay.SelectedRows[0].Cells[11].Value.ToString();
                txtOTrateperHr4.Text = dataTripPay.SelectedRows[0].Cells[12].Value.ToString();
                txtOTwage3.Text = dataTripPay.SelectedRows[0].Cells[13].Value.ToString();
                txtRegHol4.Text = dataTripPay.SelectedRows[0].Cells[14].Value.ToString();
                txtSpecHol4.Text = dataTripPay.SelectedRows[0].Cells[15].Value.ToString();
                txtSSS4.Text = dataTripPay.SelectedRows[0].Cells[16].Value.ToString();
                txtPagibig4.Text = dataTripPay.SelectedRows[0].Cells[17].Value.ToString();
                txtPhilhealth4.Text = dataTripPay.SelectedRows[0].Cells[18].Value.ToString();
                txtNet3.Text = dataTripPay.SelectedRows[0].Cells[19].Value.ToString();
                txtGross3.Text = dataTripPay.SelectedRows[0].Cells[20].Value.ToString();
                txtDeduction3.Text = dataTripPay.SelectedRows[0].Cells[21].Value.ToString();
                txt13thMonth4.Text = dataTripPay.SelectedRows[0].Cells[22].Value.ToString();
                Pickerdateofpay3.Value = dataTripPay.SelectedRows[0].Cells[23].Value == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataTripPay.SelectedRows[0].Cells[23].Value);
                txtCashAdvance2.Text = dataTripPay.SelectedRows[0].Cells[24].Value.ToString();
                txtCharges2.Text = dataTripPay.SelectedRows[0].Cells[25].Value.ToString();
                txtVat2.Text = dataTripPay.SelectedRows[0].Cells[26].Value.ToString();
            }
        }
        private void ToggleTextBoxVisibility2()
        {
            txtID3.Visible = !txtID3.Visible;
            emppID3.Visible = !emppID3.Visible;
            txtFname3.Visible = !txtFname3.Visible;
            txtLname3.Visible = !txtLname3.Visible;
            txtPosition3.Visible = !txtPosition3.Visible;
            txtAbsent3.Visible = !txtAbsent3.Visible;
            txtLeavePay3.Visible = !txtLeavePay3.Visible;
            txtpaymentMet3.Visible = !txtpaymentMet3.Visible;
            txtNoTrips3.Visible = !txtNoTrips3.Visible;
            txtRateTrip3.Visible = !txtRateTrip3.Visible;
            pnlPercent.Visible = !pnlPercent.Visible;
            txtOThour3.Visible = !txtOThour3.Visible;
            txtOTrateperHr3.Visible = !txtOTrateperHr3.Visible;
            
            txtRegHol3.Visible = !txtRegHol3.Visible;
            txtSpecHol3.Visible = !txtSpecHol3.Visible;
            txtSSS3.Visible = !txtSSS3.Visible;
            txtPagibig3.Visible = !txtPagibig3.Visible;
            txtPhilhealth3.Visible = !txtPhilhealth3.Visible;

            txt13thMonth3.Visible = !txt13thMonth3.Visible;

            txtID4.Visible = !txtID4.Visible;
            emppID4.Visible = !emppID4.Visible;
            txtFname4.Visible = !txtFname4.Visible;
            txtLname4.Visible = !txtLname4.Visible;
            txtPosition4.Visible = !txtPosition4.Visible;
            txtAbsent4.Visible = !txtAbsent4.Visible;
            txtLeavePay4.Visible = !txtLeavePay4.Visible;
            txtpaymentMet4.Visible = !txtpaymentMet4.Visible;
            txtNoTrips4.Visible = !txtNoTrips4.Visible;
            txtRateTrip4.Visible = !txtRateTrip4.Visible;

            txtOThour4.Visible = !txtOThour4.Visible;
            txtOTrateperHr4.Visible = !txtOTrateperHr4.Visible;

            txtRegHol4.Visible = !txtRegHol4.Visible;
            txtSpecHol4.Visible = !txtSpecHol4.Visible;
            txtSSS4.Visible = !txtSSS4.Visible;
            txtPagibig4.Visible = !txtPagibig4.Visible;
            txtPhilhealth4.Visible = !txtPhilhealth4.Visible;

            txt13thMonth4.Visible = !txt13thMonth4.Visible;

        }

        private void btnShowGrid_Click(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down if the grid is visible
                targetTop = this.ClientSize.Height; // Target position is off-screen (bottom)
            }
            else
            {
                // Slide up if the grid is hidden
                dataTripPay.Visible = true;  // Make the DataGridView visible before sliding up
                targetTop = 500; // Adjust this value to where you want the grid to end up (e.g., 100px from the top)
            }

            // Start the sliding animation in the appropriate direction
            SlideTimer.Start();
        }

        private void SlideTimer_Tick(object sender, EventArgs e)
        {
            // Check if the grid should slide up or down
            if (isGridVisible)
            {
                // Slide down (move to the bottom of the form)
                if (dataTripPay.Top < this.ClientSize.Height)
                {
                    dataTripPay.Top += 5;  // Adjust the speed of sliding down
                }
                else
                {
                    dataTripPay.Visible = false;  // Hide the DataGridView once it reaches the bottom
                    SlideTimer.Stop();  // Stop the timer when sliding down is complete
                    isGridVisible = false;  // Update the visibility flag
                }
            }
            else
            {
                // Slide up (move towards the targetTop position)
                if (dataTripPay.Top > targetTop)
                {
                    dataTripPay.Top -= 5;  // Adjust the speed of sliding up
                }
                else
                {
                    SlideTimer.Stop();  // Stop the timer when sliding up is complete
                    isGridVisible = true;  // Update the visibility flag
                }
            }
        }

        private void btnShowGrid2_Click(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down if the grid is visible
                targetTop = this.ClientSize.Height; // Target position is off-screen (bottom)
            }
            else
            {
                // Slide up if the grid is hidden
                dataDailyPay.Visible = true;  // Make the DataGridView visible before sliding up
                targetTop = 500; // Adjust this value to where you want the grid to end up (e.g., 100px from the top)
            }

            // Start the sliding animation in the appropriate direction
            SlideTimer1.Start();
        }

        private void SlideTimer1_Tick(object sender, EventArgs e)
        {
            if (isGridVisible)
            {
                // Slide down (move to the bottom of the form)
                if (dataDailyPay.Top < this.ClientSize.Height)
                {
                    dataDailyPay.Top += 5;  // Adjust the speed of sliding down
                }
                else
                {
                    dataDailyPay.Visible = false;  // Hide the DataGridView once it reaches the bottom
                    SlideTimer1.Stop();  // Stop the timer when sliding down is complete
                    isGridVisible = false;  // Update the visibility flag
                }
            }
            else
            {
                // Slide up (move towards the targetTop position)
                if (dataDailyPay.Top > targetTop)
                {
                    dataDailyPay.Top -= 5;  // Adjust the speed of sliding up
                }
                else
                {
                    SlideTimer1.Stop();  // Stop the timer when sliding up is complete
                    isGridVisible = true;  // Update the visibility flag
                }
            }
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            pnlPerTrip.Visible = !pnlPerTrip.Visible;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            pnlMontlyy.Visible = !pnlMontlyy.Visible;
        }

        private void btnSearch2_Click(object sender, EventArgs e)
        {
            payrollDailySearch();
        }
        private void payrollDailySearch()
        {
            dataDailyPay.Rows.Clear();

            // Retrieve the search keyword entered in txtSearch2
            string searchKeyword = txtSearch2.Text.Trim();

            // Check if the search box is empty
            if (string.IsNullOrEmpty(searchKeyword))
            {
                payrollDailyLoad();
            }

            // Open the database connection
            con.Open();

            // Modify the SQL query to filter based on the search term
            cmd = new MySqlCommand(
                "SELECT Id, EmployeeID, empFname, empLname, Position, Absent, LeavePay, paymentMethod, " +
                "noOfDays, RatePerDay, RateWage, OThours, OTrateperHour, OTrateWage, RegularHoliday, SpecialHoliday, " +
                "sssD, pagibigD, philhealthD, netPay, grossPay, TotalDeduction, thirteenMonthPay, dateofPay " +
                "FROM empprofiling " +
                "WHERE (empFname LIKE @searchKeyword OR empLname LIKE @searchKeyword OR Position LIKE @searchKeyword);", con);

            // Add the parameter for the search keyword
            cmd.Parameters.AddWithValue("@searchKeyword", "%" + searchKeyword + "%");

            rdr = cmd.ExecuteReader();

            // Populate the DataGridView with the search results
            while (rdr.Read())
            {
                dataDailyPay.Rows.Add(
                    rdr.GetInt32(0), // Id
                    rdr.GetInt32(1), // EmployeeID
                    rdr.GetString(2), // empFname
                    rdr.GetString(3), // empLname
                    rdr.GetString(4), // Position
                    rdr.GetDecimal(5), // Absent
                    rdr.GetDecimal(6), // LeavePay
                    rdr.GetString(7), // paymentMethod
                    rdr.GetDecimal(8), // noOfDays
                    rdr.GetDecimal(9), // RatePerDay
                    rdr.GetDecimal(10), // RateWage
                    rdr.GetDecimal(11), // OThours
                    rdr.GetDecimal(12), // OTrateperHour
                    rdr.GetDecimal(13), // OTrateWage
                    rdr.GetDecimal(14), // RegularHoliday
                    rdr.GetDecimal(15), // SpecialHoliday
                    rdr.GetDecimal(16), // sssD
                    rdr.GetDecimal(17), // pagibigD
                    rdr.GetDecimal(18), // philhealthD
                    rdr.GetDecimal(19), // netPay
                    rdr.GetDecimal(20), // grossPay
                    rdr.GetDecimal(21), // TotalDeduction
                    rdr.GetDecimal(22), // thirteenMonthPay
                    rdr.IsDBNull(23) ? (object)DBNull.Value : rdr.GetDateTime(23).ToString("yyyy-MM-dd") // dateofPay
                );
            }
            con.Close();
        }

        private void btnSearch3_Click(object sender, EventArgs e)
        {
            payrollTripSearch();
        }
        private void payrollTripSearch()
        {
            dataTripPay.Rows.Clear();

            // Retrieve the search keyword entered in txtSearch3
            string searchKeyword = txtSearch3.Text.Trim();

            // Check if the search box is empty
            if (string.IsNullOrEmpty(searchKeyword))
            {
                payrollTripLoad();
            }

            // Open the database connection
            con.Open();

            // Modify the SQL query to filter based on the search term
            cmd = new MySqlCommand(
                "SELECT Id, EmployeeID, empFname, empLname, Position, Absent, LeavePay, paymentMethod, " +
                "noOfTrips, PerTripRate, PerTripWage, OThours, OTrateperHour, OTrateWage, RegularHoliday, " +
                "SpecialHoliday, sssD, pagibigD, philhealthD, netPay, grossPay, TotalDeduction, thirteenMonthPay, " +
                "dateofPay " +
                "FROM empprofiling " +
                "WHERE paymentMethod = 'Per Trip' AND (empFname LIKE @searchKeyword OR empLname LIKE @searchKeyword OR Position LIKE @searchKeyword);", con);

            // Add the parameter for the search keyword
            cmd.Parameters.AddWithValue("@searchKeyword", "%" + searchKeyword + "%");

            rdr = cmd.ExecuteReader();

            // Populate the DataGridView with the search results
            while (rdr.Read())
            {
                dataTripPay.Rows.Add(
                    rdr.GetInt32(0), // Id
                    rdr.GetInt32(1), // EmployeeID
                    rdr.GetString(2), // empFname
                    rdr.GetString(3), // empLname
                    rdr.GetString(4), // Position
                    rdr.GetDecimal(5), // Absent
                    rdr.GetDecimal(6), // LeavePay
                    rdr.GetString(7), // paymentMethod
                    rdr.GetDecimal(8), // noOfTrips
                    rdr.GetDecimal(9), // PerTripRate
                    rdr.GetDecimal(10), // PerTripWage
                    rdr.GetDecimal(11), // OThours
                    rdr.GetDecimal(12), // OTrateperHour
                    rdr.GetDecimal(13), // OTrateWage
                    rdr.GetDecimal(14), // RegularHoliday
                    rdr.GetDecimal(15), // SpecialHoliday
                    rdr.GetDecimal(16), // sssD
                    rdr.GetDecimal(17), // pagibigD
                    rdr.GetDecimal(18), // philhealthD
                    rdr.GetDecimal(19), // netPay
                    rdr.GetDecimal(20), // grossPay
                    rdr.GetDecimal(21), // TotalDeduction
                    rdr.GetDecimal(22), // thirteenMonthPay
                    rdr.IsDBNull(23) ? (object)DBNull.Value : rdr.GetDateTime(23).ToString("yyyy-MM-dd") // dateofPay
                );
            }
            con.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            payrollSearchLoad();
        }
        private void payrollSearchLoad()
        {
            dataPayrollInfo.Rows.Clear();  // Clear previous data from DataGridView

            // Get the search keyword entered in txtSearch
            string searchKeyword = txtSearch.Text.Trim();

            // Ensure search term is not empty
            if (string.IsNullOrEmpty(searchKeyword))
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }

            con.Open();
            // Modify the SQL query to filter based on the search keyword
            cmd = new MySqlCommand("SELECT Id, EmployeeID, empFname, empLname, Absent, LeavePay, paymentMethod, noOfDays, RatePerDay, RateWage, " +
                                   "noOfTrips, PerTripRate, PerTripWage, OThours, OTrateperHour, OTrateWage, RegularHoliday, SpecialHoliday, " +
                                   "sssD, pagibigD, philhealthD, netPay, grossPay, TotalDeduction, thirteenMonthPay, dateofPay " +
                                   "FROM empprofiling " +
                                   "WHERE empFname LIKE @searchKeyword OR empLname LIKE @searchKeyword OR EmployeeID LIKE @searchKeyword;", con);

            // Add parameter to the query to prevent SQL injection and use the search term
            cmd.Parameters.AddWithValue("@searchKeyword", "%" + searchKeyword + "%");

            rdr = cmd.ExecuteReader();

            // Populate the DataGridView with the filtered data
            while (rdr.Read())
            {
                dataPayrollInfo.Rows.Add(
                    rdr.GetInt32(0), // Id
                    rdr.GetInt32(1), // EmployeeID
                    rdr.GetString(2), // empFname
                    rdr.GetString(3), // empLname
                    rdr.GetDecimal(4), // Absent
                    rdr.GetDecimal(5), // LeavePay
                    rdr.GetString(6), // paymentMethod
                    rdr.GetDecimal(7), // noOfDays
                    rdr.GetDecimal(8), // RatePerDay
                    rdr.GetDecimal(9), // RateWage
                    rdr.GetDecimal(10), // noOfTrips
                    rdr.GetDecimal(11), // PerTripRate
                    rdr.GetDecimal(12), // PerTripWage
                    rdr.GetDecimal(13), // OThours
                    rdr.GetDecimal(14), // OTrateperHour
                    rdr.GetDecimal(15), // OTrateWage
                    rdr.GetDecimal(16), // RegularHoliday
                    rdr.GetDecimal(17), // SpecialHoliday
                    rdr.GetDecimal(18), // sssD
                    rdr.GetDecimal(19), // pagibigD
                    rdr.GetDecimal(20), // philhealthD
                    rdr.GetDecimal(21), // netPay
                    rdr.GetDecimal(22), // grossPay
                    rdr.GetDecimal(23), // TotalDeduction
                    rdr.GetDecimal(24), // thirteenMonthPay
                    rdr.IsDBNull(25) ? (object)DBNull.Value : rdr.GetDateTime(25).ToString("yyyy-MM-dd") // dateofPay
                );
            }
            con.Close();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (!ValidateLoginInput()) return;

            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();

            var userDetails = GetUserDetails(username, password);

            if (userDetails.Item2 == "Admin")
            {
                ToggleTextBoxVisibility();
                pnlAdminLog.Visible = !pnlAdminLog.Visible;

                // Toggle the button text
                if (btnManual.Text == "To Auto")
                {
                    btnManual.Text = "To Manual";
                }
                else
                {
                    btnManual.Text = "To Auto";
                }

                ClearAdmin();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Admin Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private bool ValidateLoginInput()
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Username is Empty.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is empty.", "Error");
                return false;
            }
           
            return true;
        }
        private bool ValidateLoginInput2()
        {
            if (string.IsNullOrWhiteSpace(txtUserName2.Text))
            {
                MessageBox.Show("Username is Empty.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword2.Text))
            {
                MessageBox.Show("Password is empty.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUserName2.Text))
            {
                MessageBox.Show("Username is Empty.", "Error");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword2.Text))
            {
                MessageBox.Show("Password is empty.", "Error");
                return false;
            }
            return true;
        }
        private (string, string, byte[]) GetUserDetails(string username, string password)
        {
            try
            {
                con.Open();
                string query = "SELECT username, workLevel, Image FROM users WHERE BINARY username = @username AND BINARY password = @password";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    string retrievedUsername = rdr["username"].ToString();
                    string workLevel = rdr["workLevel"].ToString();
                    byte[] image = (byte[])rdr["Image"];
                    return (retrievedUsername, workLevel, image);
                }
                else
                {
                    return (null, null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null, null);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnEnter2_Click(object sender, EventArgs e)
        {
            if (!ValidateLoginInput2()) return;

            string username = txtUserName2.Text.Trim();
            string password = txtPassword2.Text.Trim();

            var userDetails = GetUserDetails(username, password);

            if (userDetails.Item2 == "Admin")
            {
                ToggleTextBoxVisibility2();
                pnlAdminLog2.Visible = !pnlAdminLog2.Visible;

                // Toggle the text of the button
                if (btnManual2.Text == "To Auto")
                {
                    btnManual2.Text = "To Manual";
                }
                else
                {
                    btnManual2.Text = "To Auto";
                }

                ClearAdmin();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Admin Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
          
            pnlAdminLog.Visible = !pnlAdminLog.Visible;
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {

            pnlAdminLog2.Visible = !pnlAdminLog2.Visible;
            btnManual2.Text = "AUTO";
        }

        private void chkShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPass.Checked;
        }

        private void chkShowPass2_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword2.UseSystemPasswordChar = !chkShowPass2.Checked;
        }
        private void ClearAdmin()
        {
            txtUserName.Clear();
            txtPassword.Clear();
            txtUserName2.Clear();
            txtPassword2.Clear();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            payrollinfoLoad();
        }

        private void btnPaySlip2_Click(object sender, EventArgs e)
        {
            if (dataDailyPay.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to generate the payroll receipt.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected row
            DataGridViewRow selectedRow = dataDailyPay.SelectedRows[0];

            // Create the HTML receipt content for the selected row
            StringBuilder htmlReceipt = new StringBuilder();

            htmlReceipt.AppendLine("<html>");
            htmlReceipt.AppendLine("<head><style>");
            htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
            htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
            htmlReceipt.AppendLine("h2, h3 { text-align: center; }");
            htmlReceipt.AppendLine(".section-header { margin-top: 20px; font-weight: bold; }");
            htmlReceipt.AppendLine("</style></head>");
            htmlReceipt.AppendLine("<body>");

            // Header information
            htmlReceipt.AppendLine("<h2>Payslip</h2>");
            htmlReceipt.AppendLine("<h3>JUSTIN'S CARGO SERVICES</h3>");
            htmlReceipt.AppendLine("<p>123 Company Address, City, Country</p>");
            htmlReceipt.AppendLine("<p>Contact Info: (000) 123-4567</p>");

            // Employee details
            htmlReceipt.AppendLine("<table style='width: 100%;'>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine("<td><b></b></td>");
            htmlReceipt.AppendLine($"<td><b>Employee Name:</b> {selectedRow.Cells["fnamee"].Value} {selectedRow.Cells["lnamee"].Value}</td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");

            // Handle DateTime safely
            DateTime payday = (selectedRow.Cells["payday"].Value != DBNull.Value)
                                ? Convert.ToDateTime(selectedRow.Cells["payday"].Value)
                                : default(DateTime);

            htmlReceipt.AppendLine($"<td><b>Pay Date:</b> {payday.ToString("MMMM yyyy d")}</td>");
            htmlReceipt.AppendLine("<td><b>Position:</b> " + selectedRow.Cells["positionnn"].Value?.ToString() + "</td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");

            // Handle Decimal safely
            decimal noOfDays = (selectedRow.Cells["Days"].Value != DBNull.Value)
                                ? Convert.ToDecimal(selectedRow.Cells["Days"].Value)
                                : 0m;

            decimal ratePerDay = (selectedRow.Cells["rateperday"].Value != DBNull.Value)
                                  ? Convert.ToDecimal(selectedRow.Cells["rateperday"].Value)
                                  : 0m;

            htmlReceipt.AppendLine($"<td><b>No. of Days:</b> {noOfDays}</td>");
            htmlReceipt.AppendLine($"<td><b>Rate per Day:</b> {ratePerDay.ToString("C2")}</td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("</table>");

            // Wages section
            htmlReceipt.AppendLine("<h3 class='section-header'>Wages</h3>");
            htmlReceipt.AppendLine("<table style='width: 100%;'>");
            htmlReceipt.AppendLine("<tr>");

            // Handle Decimal safely
            decimal perDayWage = (selectedRow.Cells["ratewage"].Value != DBNull.Value)
                                  ? Convert.ToDecimal(selectedRow.Cells["ratewage"].Value)
                                  : 0m;

            decimal overtimeWage = (selectedRow.Cells["otwage"].Value != DBNull.Value)
                                    ? Convert.ToDecimal(selectedRow.Cells["otwage"].Value)
                                    : 0m;

            decimal grossPay = (selectedRow.Cells["grossPay"].Value != DBNull.Value)
                               ? Convert.ToDecimal(selectedRow.Cells["grossPay"].Value)
                               : 0m;

            decimal netPay = (selectedRow.Cells["netPay"].Value != DBNull.Value)
                             ? Convert.ToDecimal(selectedRow.Cells["netPay"].Value)
                             : 0m;

            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Per Day Wage:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{perDayWage.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Overtime Wage:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{overtimeWage.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Gross Pay:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{grossPay.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Net Pay:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{netPay.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("</table>");

            // Deductions section
            htmlReceipt.AppendLine("<h3 class='section-header'>Deductions</h3>");
            htmlReceipt.AppendLine("<table>");
            htmlReceipt.AppendLine("<tr><th>Deductions</th><th>Amount</th></tr>");

            // Handle Decimal safely
            decimal sssDeduction = (selectedRow.Cells["sssD"].Value != DBNull.Value)
                                   ? Convert.ToDecimal(selectedRow.Cells["sssD"].Value)
                                   : 0m;

            decimal pagibigDeduction = (selectedRow.Cells["pagibigD"].Value != DBNull.Value)
                                       ? Convert.ToDecimal(selectedRow.Cells["pagibigD"].Value)
                                       : 0m;

            decimal philHealthDeduction = (selectedRow.Cells["philhealthD"].Value != DBNull.Value)
                                          ? Convert.ToDecimal(selectedRow.Cells["philhealthD"].Value)
                                          : 0m;

            decimal totalDeductions = (selectedRow.Cells["TotalDeduction"].Value != DBNull.Value)
                                      ? Convert.ToDecimal(selectedRow.Cells["TotalDeduction"].Value)
                                      : 0m;

            htmlReceipt.AppendLine($"<tr><td>SSS Deduction</td><td>{sssDeduction.ToString("C2")}</td></tr>");
            htmlReceipt.AppendLine($"<tr><td>Pag-IBIG Deduction</td><td>{pagibigDeduction.ToString("C2")}</td></tr>");
            htmlReceipt.AppendLine($"<tr><td>PhilHealth Deduction</td><td>{philHealthDeduction.ToString("C2")}</td></tr>");
            htmlReceipt.AppendLine($"<tr><td><b>Total Deductions</b></td><td><b>{totalDeductions.ToString("C2")}</b></td></tr>");
            htmlReceipt.AppendLine("</table>");

            htmlReceipt.AppendLine("</body>");
            htmlReceipt.AppendLine("</html>");

            // Convert the HTML content to a List<string> for GeneratePdf
            var htmlContents = new List<string> { htmlReceipt.ToString() };

            // Call the GeneratePdf function to generate the PDF from the HTML
            GeneratePdf(htmlContents);
            insertDailryRecords();
        }

        private void insertDailryRecords()
        {
            con.Open();

            // Construct the INSERT query
            cmd = new MySqlCommand(
                "INSERT INTO payrollrecords (" +
                "EmployeeID, empFname, empLname,  RateWage, " +
                "RegularHoliday, SpecialHoliday, CashAdvance, Vat, Charges, OThours, OTrateperHour, " +
                "OTrateWage, sssD, pagibigD, philhealthD, netPay, grossPay, TotalDeduction, payDate) " +
                "VALUES (" +
                "@EmployeeID, @empFname, @empLname, , @RateWage, " +
                "@RegularHoliday, @SpecialHoliday, @CashAdvance, @Vat, @Charges, @OThours, @OTrateperHour, " +
                "@OTrateWage, @sssD, @pagibigD, @philhealthD, @netPay, @grossPay, @TotalDeduction, @payDate)", con);

            // Add parameters for INSERT
            cmd.Parameters.AddWithValue("@EmployeeID", emppID.Text);
            cmd.Parameters.AddWithValue("@empFname", txtFname.Text);
            cmd.Parameters.AddWithValue("@empLname", txtLname.Text);
           
           
            cmd.Parameters.AddWithValue("@RatePerDay", txtratePday.Text);
            cmd.Parameters.AddWithValue("@RateWage", txtrateWage.Text);
            cmd.Parameters.AddWithValue("@RegularHoliday", txtRegHol.Text);
            cmd.Parameters.AddWithValue("@SpecialHoliday", txtSpecHol.Text);
            cmd.Parameters.AddWithValue("@CashAdvance", txtCashAdvance.Text);
            cmd.Parameters.AddWithValue("@Vat", txtVat.Text);
            cmd.Parameters.AddWithValue("@Charges", txtCharges6.Text);
            cmd.Parameters.AddWithValue("@OThours", txtOThour.Text);
            cmd.Parameters.AddWithValue("@OTrateperHour", txtOTrateperHr.Text);
            cmd.Parameters.AddWithValue("@OTrateWage", txtOTwage.Text);
            cmd.Parameters.AddWithValue("@sssD", txtSSS.Text);
            cmd.Parameters.AddWithValue("@pagibigD", txtPagibig.Text);
            cmd.Parameters.AddWithValue("@philhealthD", txtPhilhealth.Text);
            cmd.Parameters.AddWithValue("@netPay", txtNet.Text);
            cmd.Parameters.AddWithValue("@grossPay", txtGross.Text);
            cmd.Parameters.AddWithValue("@TotalDeduction", txtDeduction.Text);
            cmd.Parameters.AddWithValue("@payDate", DateTime.Now); // Replace with the appropriate pay date if needed

            // Execute the INSERT query
            cmd.ExecuteNonQuery();

            con.Close();
            MessageBox.Show("Record Inserted Successfully!", "Success");

            // Call subsequent methods
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateHalfDayDeductions();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            hidecoloumns();
        }
        private void btnPaySlip3_Click(object sender, EventArgs e)
        {
            if (dataTripPay.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to generate the payroll receipt.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected row
            DataGridViewRow selectedRow = dataTripPay.SelectedRows[0];

            // Create the HTML receipt content for the selected row
            StringBuilder htmlReceipt = new StringBuilder();

            htmlReceipt.AppendLine("<html>");
            htmlReceipt.AppendLine("<head><style>");
            htmlReceipt.AppendLine("table { border-collapse: collapse; width: 100%; }");
            htmlReceipt.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            htmlReceipt.AppendLine("th { background-color: #f2f2f2; }");
            htmlReceipt.AppendLine("h2, h3 { text-align: center; }");
            htmlReceipt.AppendLine(".section-header { margin-top: 20px; font-weight: bold; }");
            htmlReceipt.AppendLine("</style></head>");
            htmlReceipt.AppendLine("<body>");

            // Header information
            htmlReceipt.AppendLine("<h2>Payslip</h2>");
            htmlReceipt.AppendLine("<h3>JUSTIN'S CARGO SERVICES</h3>");
            htmlReceipt.AppendLine("<p>123 Company Address, City, Country</p>");
            htmlReceipt.AppendLine("<p>Contact Info: (000) 123-4567</p>");

            // Employee details
            htmlReceipt.AppendLine("<table style='width: 100%;'>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine("<td><b></b></td>");
            htmlReceipt.AppendLine($"<td><b>Employee Name:</b> {selectedRow.Cells["empFname"].Value} {selectedRow.Cells["empLname"].Value}</td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");

            // Handle DateTime safely
            DateTime payDate = (selectedRow.Cells["PAYDATE"].Value != DBNull.Value)
                                ? Convert.ToDateTime(selectedRow.Cells["PAYDATE"].Value)
                                : default(DateTime);
            htmlReceipt.AppendLine($"<td><b>Pay Date:</b> {payDate.ToString("MMMM yyyy d")}</td>");
            htmlReceipt.AppendLine("<td><b>Position:</b> " + selectedRow.Cells["Position"].Value?.ToString() + "</td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");

            // Handle decimals safely
            decimal noOfTrips = (selectedRow.Cells["NOTRIPS"].Value != DBNull.Value)
                                ? Convert.ToDecimal(selectedRow.Cells["NOTRIPS"].Value)
                                : 0m;

            decimal ratePerTrip = (selectedRow.Cells["RATET"].Value != DBNull.Value)
                                  ? Convert.ToDecimal(selectedRow.Cells["RATET"].Value)
                                  : 0m;

            htmlReceipt.AppendLine($"<td><b>No. of Trips:</b> {noOfTrips}</td>");
            htmlReceipt.AppendLine($"<td><b>Rate per Trip:</b> ₱{ratePerTrip.ToString("C2")}</td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("</table>");

            // Wages section
            htmlReceipt.AppendLine("<h3 class='section-header'>Wages</h3>");
            htmlReceipt.AppendLine("<table style='width: 100%;'>");

            // Handle Decimal wages safely
            decimal perTripWage = (selectedRow.Cells["RATEW"].Value != DBNull.Value)
                                  ? Convert.ToDecimal(selectedRow.Cells["RATEW"].Value)
                                  : 0m;

            decimal overtimeWage = (selectedRow.Cells["OTWAGEE"].Value != DBNull.Value)
                                    ? Convert.ToDecimal(selectedRow.Cells["OTWAGEE"].Value)
                                    : 0m;

            decimal grossPay = (selectedRow.Cells["GROSSS"].Value != DBNull.Value)
                               ? Convert.ToDecimal(selectedRow.Cells["GROSSS"].Value)
                               : 0m;

            decimal netPay = (selectedRow.Cells["NETTT"].Value != DBNull.Value)
                             ? Convert.ToDecimal(selectedRow.Cells["NETTT"].Value)
                             : 0m;

            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Per Trip Wage:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{perTripWage.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Overtime Wage:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{overtimeWage.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Gross Pay:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{grossPay.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("<tr>");
            htmlReceipt.AppendLine($"<td style='text-align: left;'><b>Net Pay:</b></td>");
            htmlReceipt.AppendLine($"<td style='text-align: right;'><b>₱{netPay.ToString("C2")}</b></td>");
            htmlReceipt.AppendLine("</tr>");
            htmlReceipt.AppendLine("</table>");

            // Deductions section
            htmlReceipt.AppendLine("<h3 class='section-header'>Deductions</h3>");
            htmlReceipt.AppendLine("<table>");
            htmlReceipt.AppendLine("<tr><th>Deductions</th><th>Amount</th></tr>");

            // Handle Decimal deductions safely
            decimal sssDeduction = (selectedRow.Cells["DSSS"].Value != DBNull.Value)
                                   ? Convert.ToDecimal(selectedRow.Cells["DSSS"].Value)
                                   : 0m;

            decimal pagibigDeduction = (selectedRow.Cells["DPAGIBIG"].Value != DBNull.Value)
                                       ? Convert.ToDecimal(selectedRow.Cells["DPAGIBIG"].Value)
                                       : 0m;

            decimal philHealthDeduction = (selectedRow.Cells["DPHILHEALTH"].Value != DBNull.Value)
                                          ? Convert.ToDecimal(selectedRow.Cells["DPHILHEALTH"].Value)
                                          : 0m;

            decimal totalDeductions = (selectedRow.Cells["TOTALD"].Value != DBNull.Value)
                                      ? Convert.ToDecimal(selectedRow.Cells["TOTALD"].Value)
                                      : 0m;

            htmlReceipt.AppendLine($"<tr><td>SSS Deduction</td><td>{sssDeduction.ToString("C2")}</td></tr>");
            htmlReceipt.AppendLine($"<tr><td>Pag-IBIG Deduction</td><td>{pagibigDeduction.ToString("C2")}</td></tr>");
            htmlReceipt.AppendLine($"<tr><td>PhilHealth Deduction</td><td>{philHealthDeduction.ToString("C2")}</td></tr>");
            htmlReceipt.AppendLine($"<tr><td><b>Total Deductions</b></td><td><b>{totalDeductions.ToString("C2")}</b></td></tr>");
            htmlReceipt.AppendLine("</table>");

            htmlReceipt.AppendLine("</body>");
            htmlReceipt.AppendLine("</html>");

            // Convert the HTML content to a List<string> for GeneratePdf
            var htmlContents = new List<string> { htmlReceipt.ToString() };

            // Call the GeneratePdf function to generate the PDF from the HTML
            GeneratePdf(htmlContents);
            insertTripRecords();

        }
        private void insertTripRecords()
        {
            con.Open();

            // Construct the INSERT query without RestDay and TotalHolidayPay
            cmd = new MySqlCommand(
                "INSERT INTO payrollrecords (" +
                "EmployeeID, empFname, empLname, RateWage, PerTripWage, OTrateWage, RegularHoliday, " +
                "SpecialHoliday, CashAdvance, Vat, Charges, sssD, pagibigD, philhealthD, netPay, " +
                "grossPay, TotalDeduction, payDate) " +
                "VALUES (" +
                "@EmployeeID, @empFname, @empLname, @RateWage, @PerTripWage, @OTrateWage, @RegularHoliday, " +
                "@SpecialHoliday, @CashAdvance, @Vat, @Charges, @sssD, @pagibigD, @philhealthD, " +
                "@netPay, @grossPay, @TotalDeduction, @payDate)", con);

            // Add parameters for INSERT
            cmd.Parameters.AddWithValue("@EmployeeID", emppID3.Text);
            cmd.Parameters.AddWithValue("@empFname", txtFname3.Text);
            cmd.Parameters.AddWithValue("@empLname", txtLname3.Text);
            cmd.Parameters.AddWithValue("@RateWage", txtrateWage3.Text);
            cmd.Parameters.AddWithValue("@PerTripWage", txtRateTrip3.Text);
            cmd.Parameters.AddWithValue("@OTrateWage", txtOTwage3.Text);
            cmd.Parameters.AddWithValue("@RegularHoliday", txtRegHol3.Text);
            cmd.Parameters.AddWithValue("@SpecialHoliday", txtSpecHol3.Text);
            cmd.Parameters.AddWithValue("@CashAdvance", txtCashAdvance2.Text);
            cmd.Parameters.AddWithValue("@Vat", txtVat2.Text);
            cmd.Parameters.AddWithValue("@Charges", txtCharges2.Text);
            cmd.Parameters.AddWithValue("@sssD", txtSSS3.Text);
            cmd.Parameters.AddWithValue("@pagibigD", txtPagibig3.Text);
            cmd.Parameters.AddWithValue("@philhealthD", txtPhilhealth3.Text);
            cmd.Parameters.AddWithValue("@netPay", txtNet3.Text);
            cmd.Parameters.AddWithValue("@grossPay", txtGross3.Text);
            cmd.Parameters.AddWithValue("@TotalDeduction", txtDeduction3.Text);
            cmd.Parameters.AddWithValue("@payDate", DateTime.Now); // Replace with appropriate date if needed

            // Execute the INSERT query
            cmd.ExecuteNonQuery();

            con.Close();
            MessageBox.Show("Record Inserted Successfully!", "Success");

            // Call subsequent methods
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateHalfDayDeductions();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            hidecoloumns();
        }
        private void GeneratePdf(List<string> htmlContents)
        {
            try
            {
                var converter = new BasicConverter(new PdfTools()); // Initialize the converter outside the loop

                foreach (var htmlContent in htmlContents)
                {
                    var doc = new HtmlToPdfDocument()
                    {
                        GlobalSettings = {
                    ColorMode = DinkToPdf.ColorMode.Color,
                    Orientation = DinkToPdf.Orientation.Landscape,
                    PaperSize = DinkToPdf.PaperKind.A4
                },
                        Objects = {
                    new DinkToPdf.ObjectSettings() {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
                    };

                    // Generate the PDF
                    byte[] pdf = converter.Convert(doc);

                    // Save and open the PDF with a unique name
                    SaveAndOpenPdf(pdf);

                    // Cleanup HtmlToPdfDocument explicitly
                    doc = null;
                }

                // Cleanup the converter explicitly
                converter = null;
                GC.Collect(); // Force garbage collection
                GC.WaitForPendingFinalizers();
            }
            catch (AccessViolationException ex)
            {
                MessageBox.Show($"Access violation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveAndOpenPdf(byte[] pdfContent)
        {
            try
            {
                // Specify the download directory
                string downloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                // Generate a unique file name using a timestamp and GUID
                string uniqueName = $"GeneratedReport_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}.pdf";
                string filePath = Path.Combine(downloadDirectory, uniqueName);

                // Save the file
                File.WriteAllBytes(filePath, pdfContent);

                MessageBox.Show($"PDF DOWNLOADED", "SUCCESS", MessageBoxButtons.OK);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving/opening PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            pnlShowPayOp.Visible = !pnlShowPayOp.Visible;
        }

        private void btnAdditionalD_Click(object sender, EventArgs e)
        {
            pnladditonalD1.Visible = !pnladditonalD1.Visible;
        }

        private void btnAdditonalD2_Click(object sender, EventArgs e)
        {
            pnlAdditonalD2.Visible = !pnlAdditonalD2.Visible;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Connection string (adjust to your environment)
            string connectionString = "datasource=localhost;" +
                                      "port=3306;" +
                                      "database=JCSdb;" +
                                      "username=root;" +
                                      "password=;" +
                                      "Convert Zero Datetime=True;";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    // Define the SQL update query
                    string query = @"
                UPDATE empprofiling 
                SET 
                    from_ = @FromDate,
                    to_ = @ToDate,
                    sssDrate = @SSSRate,
                    philhealthDrate = @PhilHealthRate";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@FromDate", txtFrom.Text);
                        cmd.Parameters.AddWithValue("@ToDate", txtTo.Text);

                        // Validate and parse decimal values
                        if (decimal.TryParse(txtSSSP.Text, out decimal sssRate))
                        {
                            cmd.Parameters.AddWithValue("@SSSRate", sssRate);
                        }
                        else
                        {
                            MessageBox.Show("Invalid SSS Rate. Please enter a valid decimal number.");
                            return;
                        }

                        if (decimal.TryParse(txtPHP.Text, out decimal philhealthRate))
                        {
                            cmd.Parameters.AddWithValue("@PhilHealthRate", philhealthRate);
                        }
                        else
                        {
                            MessageBox.Show("Invalid PhilHealth Rate. Please enter a valid decimal number.");
                            return;
                        }

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Notify the user
                        MessageBox.Show($"{rowsAffected} record(s) updated successfully.", "Success");
             
                        CountEmployeeDays();
                        countTrips();
                        CalculateAndUpdateOvertimeForCurrentMonth();
                        CalculateAndUpdateAbsencesForCurrentMonth();
                        CalculateHolidayPay();
                        UpdateEmployeePayments();
                        CalculateAndUpdate13thMonthPayForCurrentYear();
                        CountEmployeeDaysforthismonth();
                        payrollinfoLoad();
                        payrollrecordsLoad();
                        payrollTripLoad();
                        payrollDailyLoad();
                        payrollPercentageLoad2();
                        payrollPercentageLoad();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            // Connection string (adjust to your environment)
            string connectionString = "datasource=localhost;" +
                                      "port=3306;" +
                                      "database=JCSdb;" +
                                      "username=root;" +
                                      "password=;" +
                                      "Convert Zero Datetime=True;";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    // Define the SQL update query
                    string query = @"
                UPDATE empprofiling 
                SET 
                    from_ = @FromDate,
                    to_ = @ToDate,
                    sssDrate = @SSSRate,
                    philhealthDrate = @PhilHealthRate";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@FromDate", txtFrom2.Text);
                        cmd.Parameters.AddWithValue("@ToDate", txtTo2.Text);

                        // Validate and parse decimal values
                        if (decimal.TryParse(sssDP.Text, out decimal sssRate))
                        {
                            cmd.Parameters.AddWithValue("@SSSRate", sssRate);
                        }
                        else
                        {
                            MessageBox.Show("Invalid SSS Rate. Please enter a valid decimal number.");
                            return;
                        }

                        if (decimal.TryParse(txtphilhealthP.Text, out decimal philhealthRate))
                        {
                            cmd.Parameters.AddWithValue("@PhilHealthRate", philhealthRate);
                        }
                        else
                        {
                            MessageBox.Show("Invalid PhilHealth Rate. Please enter a valid decimal number.");
                            return;
                        }

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Notify the user
                        MessageBox.Show($"{rowsAffected} record(s) updated successfully.", "Success");

                        CountEmployeeDays();
                        countTrips();
                        CalculateAndUpdateOvertimeForCurrentMonth();
                        CalculateAndUpdateAbsencesForCurrentMonth();
                        CalculateHolidayPay();
                        UpdateEmployeePayments();
                        CalculateAndUpdate13thMonthPayForCurrentYear();
                        CountEmployeeDaysforthismonth();
                        payrollinfoLoad();
                        payrollrecordsLoad();
                        payrollTripLoad();
                        payrollDailyLoad();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }

        private void dataratePecentage_SelectionChanged(object sender, EventArgs e)
        {
            if (dataratePecentage.SelectedRows.Count > 0)
            {
                txtIDDD.Text = dataratePecentage.SelectedRows[0].Cells[0].Value.ToString();
                txtFrom2.Text = dataratePecentage.SelectedRows[0].Cells[1].Value.ToString();
                txtTo2.Text = dataratePecentage.SelectedRows[0].Cells[2].Value.ToString();
                sssDP.Text = dataratePecentage.SelectedRows[0].Cells[3].Value.ToString();
                txtphilhealthP.Text = dataratePecentage.SelectedRows[0].Cells[4].Value.ToString();
                
            }
        }

        private void btnUpdatee_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand("UPDATE ratepercentage SET  " +
                "from_ =@from_ , " +
                "to_ = @to_ , " +
                "sssPercentage = @sssPercentage, " +
                "philHealthrate = @philHealthrate " +
                "WHERE ID=@EmployeeID", con);


            cmd.Parameters.AddWithValue("@EmployeeID", txtIDDD.Text);
            cmd.Parameters.AddWithValue("@from_", txtFrom2.Text);
            cmd.Parameters.AddWithValue("@to_", txtTo2.Text);
            cmd.Parameters.AddWithValue("@sssPercentage", sssDP.Text);
            cmd.Parameters.AddWithValue("@philHealthrate", txtphilhealthP.Text);
            

            cmd.ExecuteNonQuery();



            con.Close();
            MessageBox.Show("Record Updated Successfully!", "Success");
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            payrollPercentageLoad();
        }

        private void ShowGrid_Click(object sender, EventArgs e)
        {
            dataratePecentage.Visible = !dataratePecentage.Visible;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand("INSERT INTO ratepercentage " +
                "(from_, to_, sssPercentage, philHealthrate) " +
                "VALUES (@from_, @to_, @sssPercentage, @philHealthrate)", con);

            cmd.Parameters.AddWithValue("@from_", txtFrom2.Text);
            cmd.Parameters.AddWithValue("@to_", txtTo2.Text);
            cmd.Parameters.AddWithValue("@sssPercentage", sssDP.Text);
            cmd.Parameters.AddWithValue("@philHealthrate", txtphilhealthP.Text);

            cmd.ExecuteNonQuery();

            con.Close();
            MessageBox.Show("Record Inserted Successfully!", "Success");
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            payrollPercentageLoad();
        }

        private void btnUpdateee_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand("UPDATE ratepercentage SET  " +
                "from_ =@from_ , " +
                "to_ = @to_ , " +
                "sssPercentage = @sssPercentage, " +
                "philHealthrate = @philHealthrate " +
                "WHERE ID=@EmployeeID", con);


            cmd.Parameters.AddWithValue("@EmployeeID", txtIDDDD.Text);
            cmd.Parameters.AddWithValue("@from_", txtFrom.Text);
            cmd.Parameters.AddWithValue("@to_", txtTo.Text);
            cmd.Parameters.AddWithValue("@sssPercentage", txtSSSP.Text);
            cmd.Parameters.AddWithValue("@philHealthrate", txtPHP.Text);


            cmd.ExecuteNonQuery();



            con.Close();
            MessageBox.Show("Record Updated Successfully!", "Success");
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            payrollPercentageLoad2();
        }

        private void btnAdd2_Click(object sender, EventArgs e)
        {
            con.Open();
            cmd = new MySqlCommand("INSERT INTO ratepercentage " +
                "(from_, to_, sssPercentage, philHealthrate) " +
                "VALUES (@from_, @to_, @sssPercentage, @philHealthrate)", con);

            cmd.Parameters.AddWithValue("@from_", txtFrom.Text);
            cmd.Parameters.AddWithValue("@to_", txtTo.Text);
            cmd.Parameters.AddWithValue("@sssPercentage", txtSSSP.Text);
            cmd.Parameters.AddWithValue("@philHealthrate", txtPHP.Text);

            cmd.ExecuteNonQuery();

            con.Close();
            MessageBox.Show("Record Inserted Successfully!", "Success");
            CountEmployeeDays();
            countTrips();
            CalculateAndUpdateOvertimeForCurrentMonth();
            CalculateAndUpdateAbsencesForCurrentMonth();
            CalculateHolidayPay();
            UpdateEmployeePayments();
            CalculateAndUpdate13thMonthPayForCurrentYear();
            CountEmployeeDaysforthismonth();
            payrollinfoLoad();
            payrollrecordsLoad();
            payrollTripLoad();
            payrollDailyLoad();
            payrollPercentageLoad2();

        }

        private void btnShowGridd_Click(object sender, EventArgs e)
        {
            dataPercentagerate2.Visible = !dataPercentagerate2.Visible;
        }

        private void dataPercentagerate2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataPercentagerate2.SelectedRows.Count > 0)
            {
                txtIDDDD.Text = dataPercentagerate2.SelectedRows[0].Cells[0].Value.ToString();
                txtFrom.Text = dataPercentagerate2.SelectedRows[0].Cells[1].Value.ToString();
                txtTo.Text = dataPercentagerate2.SelectedRows[0].Cells[2].Value.ToString();
                txtSSSP.Text = dataPercentagerate2.SelectedRows[0].Cells[3].Value.ToString();
                txtPHP.Text = dataPercentagerate2.SelectedRows[0].Cells[4].Value.ToString();

            }
        }

        private void btnPrint13thMonth_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataPayrollRecords.Rows.Count == 0)
                {
                    MessageBox.Show("No data to print.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Build the table rows for the DataGridView contents
                var rows = new List<string>();
                decimal totalNetPay = 0;

                foreach (DataGridViewRow row in dataPayrollRecords.Rows)
                {
                    if (row.Cells["NET"].Value != null)
                    {
                        decimal netPay = Convert.ToDecimal(row.Cells["NET"].Value);
                        totalNetPay += netPay; // Sum the netPay values
                    }

                    // Construct HTML row for this record
                    rows.Add($@"
                <tr>
                    <td>{row.Cells["key"].Value}</td>
                    <td>{row.Cells["EMPLOYEE"].Value}</td>
                    <td>{row.Cells["FNAME"].Value} {row.Cells["LNAME"].Value}</td>
                    <td>{row.Cells["RW"].Value}</td>
                    <td>{row.Cells["PTW"].Value}</td>
                    <td>{row.Cells["OT"].Value}</td>
                    <td>{row.Cells["SSS"].Value}</td>
                    <td>{row.Cells["PAGIBIG"].Value}</td>
                    <td>{row.Cells["PH"].Value}</td>
                    <td>{row.Cells["NET"].Value}</td>
                    <td>{row.Cells["GROSS"].Value}</td>
                    <td>{row.Cells["TD"].Value}</td>
                    <td>{row.Cells["PD"].Value}</td>
                </tr>");
                }

                // Calculate 13th Month Pay
                decimal calculatedThirteenthMonthPay = totalNetPay / 12;

                // Generate HTML content
                string htmlContent = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    h1 {{ text-align: center; }}
                    table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
                    th, td {{ border: 1px solid black; padding: 8px; text-align: left; }}
                    th {{ background-color: #f2f2f2; }}
                    .footer {{ text-align: right; font-size: 1.2em; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <h1>Payroll Report</h1>
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Employee ID</th>
                            <th>Name</th>
                            <th>Rate Wage</th>
                            <th>Per Trip Wage</th>
                            <th>OT Rate Wage</th>
                            <th>SSS Deduction</th>
                            <th>PAG-IBIG Deduction</th>
                            <th>PhilHealth Deduction</th>
                            <th>Net Pay</th>
                            <th>Gross Pay</th>
                            <th>Total Deduction</th>
                            <th>Pay Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {string.Join("", rows)}
                    </tbody>
                </table>
                <div class='footer'>
                    <strong>Total Net Pay: ₱{totalNetPay:N2}</strong><br>
                    <strong>13th Month Pay: ₱{calculatedThirteenthMonthPay:N2}</strong>
                </div>
            </body>
            </html>";

                // Convert HTML to PDF
                var converter = new BasicConverter(new PdfTools());
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                ColorMode = DinkToPdf.ColorMode.Color,
                Orientation = DinkToPdf.Orientation.Landscape,
                PaperSize = DinkToPdf.PaperKind.A4
            },
                    Objects = {
                new DinkToPdf.ObjectSettings()
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
                };

                byte[] pdf = converter.Convert(doc);

                // Save and open the PDF
                SaveAndOpenPdf(pdf);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating the report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PrintPayrollReportWith13thMonthPay(int employeeID, decimal thirteenthMonthPay)
        {
            
        }

       

        private void SortemployeeID_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (SortemployeeID.SelectedItem != null)
            {
                int selectedEmployeeID = Convert.ToInt32(SortemployeeID.SelectedItem.ToString());
                LoadPayrollRecords(selectedEmployeeID); // Call method to load records for the selected employee
            }
            else
            {
                MessageBox.Show("Please select a valid employee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void btn13th_Click(object sender, EventArgs e)
        {
            pnlMontlyy.Visible = !pnlMontlyy.Visible;
            pnl13thMonthPay.Visible = !pnl13thMonthPay.Visible;

        }

        private void btn13thh_Click(object sender, EventArgs e)
        {
            pnl13thMonthPay.Visible = !pnl13thMonthPay.Visible;
            pnlPerTrip.Visible = !pnlPerTrip.Visible;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            pnl13thMonthPay.Visible = !pnl13thMonthPay.Visible;
        }
    }

        
    }
    


