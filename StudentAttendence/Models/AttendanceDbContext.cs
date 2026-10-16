using System.Data.Entity;

namespace StudentAttendence.Models
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext() : base("AttendanceDB") { }

        public DbSet<Student>            Students            { get; set; }
        public DbSet<Attendance>         Attendances         { get; set; }
        public DbSet<LocationSetting>    LocationSettings    { get; set; }
        public DbSet<Subject>            Subjects            { get; set; }
        public DbSet<AcademicStream>     Streams             { get; set; }
        public DbSet<AcademicDepartment> Departments         { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    /// <summary>
    /// Safe initializer:
    ///   - Creates the DB if it doesn't exist yet.
    ///   - Adds any new columns / tables to an existing DB without dropping data.
    /// </summary>
    public class AttendanceDbInitializer : IDatabaseInitializer<AttendanceDbContext>
    {
        public void InitializeDatabase(AttendanceDbContext context)
        {
            if (!context.Database.Exists())
            {
                context.Database.Create();
                return;           // brand-new DB — EF already created every table
            }

            // ── Students: face columns ────────────────────────────────────────
            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns
                    WHERE Name='PhotoBase64' AND Object_ID=Object_ID('Students'))
                ALTER TABLE Students ADD PhotoBase64 NVARCHAR(MAX) NULL;");

            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns
                    WHERE Name='FaceDescriptor' AND Object_ID=Object_ID('Students'))
                ALTER TABLE Students ADD FaceDescriptor NVARCHAR(MAX) NULL;");

            // ── Students: Support RollNo column rename in database if present ─
            context.Database.ExecuteSqlCommand(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE Name='PRN' AND Object_ID=Object_ID('Students'))
                   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name='RollNo' AND Object_ID=Object_ID('Students'))
                BEGIN
                    EXEC sp_rename 'Students.PRN', 'RollNo', 'COLUMN';
                END");

            // ── LocationSettings table ────────────────────────────────────────
            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name='LocationSettings')
                CREATE TABLE LocationSettings (
                    Id          INT IDENTITY(1,1) PRIMARY KEY,
                    PlaceName   NVARCHAR(200) NOT NULL,
                    Latitude    FLOAT         NOT NULL,
                    Longitude   FLOAT         NOT NULL,
                    RadiusMeters INT          NOT NULL DEFAULT 200,
                    UpdatedOn   DATETIME      NOT NULL DEFAULT GETDATE()
                );");

            // ── Streams & Departments tables ──────────────────────────────────
            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name='Streams')
                CREATE TABLE Streams (
                    StreamId   INT IDENTITY(1,1) PRIMARY KEY,
                    StreamName NVARCHAR(100) NOT NULL
                );

                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name='Departments')
                CREATE TABLE Departments (
                    DepartmentId   INT IDENTITY(1,1) PRIMARY KEY,
                    DepartmentName NVARCHAR(150) NOT NULL,
                    StreamId       INT NOT NULL FOREIGN KEY REFERENCES Streams(StreamId) ON DELETE CASCADE
                );");

            // ── Seed Streams & Departments if not present ─────────────────────
            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM Streams)
                BEGIN
                    -- 1. Arts
                    INSERT INTO Streams (StreamName) VALUES ('Arts');
                    DECLARE @ArtsId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('English', @ArtsId),
                    ('Marathi', @ArtsId),
                    ('Hindi', @ArtsId),
                    ('History', @ArtsId),
                    ('Geography', @ArtsId);

                    -- 2. Commerce
                    INSERT INTO Streams (StreamName) VALUES ('Commerce');
                    DECLARE @CommerceId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('Accountancy & Taxation', @CommerceId),
                    ('Marketing', @CommerceId),
                    ('Cost & Management Accounting', @CommerceId),
                    ('Banking & Finance', @CommerceId),
                    ('Business Statistics', @CommerceId);

                    -- 3. Diploma
                    INSERT INTO Streams (StreamName) VALUES ('Diploma');
                    DECLARE @DiplomaId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('PG Diploma in Taxation Laws', @DiplomaId);

                    -- 4. Science
                    INSERT INTO Streams (StreamName) VALUES ('Science');
                    DECLARE @ScienceId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('Mathematics', @ScienceId),
                    ('Physics', @ScienceId),
                    ('Botany', @ScienceId),
                    ('Zoology', @ScienceId),
                    ('Electronics', @ScienceId),
                    ('Computer Science', @ScienceId),
                    ('Chemistry', @ScienceId),
                    ('Food Technology', @ScienceId),
                    ('Data Science', @ScienceId);

                    -- 5. PHD
                    INSERT INTO Streams (StreamName) VALUES ('PHD');
                    DECLARE @PHDId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('Botany', @PHDId),
                    ('Economics', @PHDId),
                    ('Hindi', @PHDId),
                    ('Physics', @PHDId),
                    ('Marathi', @PHDId),
                    ('Statistics', @PHDId),
                    ('Commerce & Management', @PHDId),
                    ('Microbiology', @PHDId);

                    -- 6. Vocational
                    INSERT INTO Streams (StreamName) VALUES ('Vocational');
                    DECLARE @VocationalId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('Food Processing & Post Harvest Technology', @VocationalId),
                    ('Journalism & Mass Communication', @VocationalId),
                    ('Retail Management', @VocationalId),
                    ('Dairy Technology', @VocationalId),
                    ('E-Commerce & Digital Marketing', @VocationalId),
                    ('Media Studies', @VocationalId);

                    -- 7. Professional
                    INSERT INTO Streams (StreamName) VALUES ('Professional');
                    DECLARE @ProfessionalId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('BBA', @ProfessionalId),
                    ('BBA(CA)', @ProfessionalId),
                    ('BCA', @ProfessionalId);

                    -- 8. Others
                    INSERT INTO Streams (StreamName) VALUES ('Others');
                    DECLARE @OthersId INT = SCOPE_IDENTITY();
                    INSERT INTO Departments (DepartmentName, StreamId) VALUES
                    ('Bachelor of Library & Info Science', @OthersId),
                    ('Master of Library & Info Science', @OthersId);
                END");

            // ── Subjects table ────────────────────────────────────────────────
            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name='Subjects')
                CREATE TABLE Subjects (
                    SubjectId             INT IDENTITY(1,1) PRIMARY KEY,
                    SubjectName           NVARCHAR(100) NOT NULL,
                    SubjectCode           NVARCHAR(50)  NOT NULL,
                    Department            NVARCHAR(100) NULL,
                    Stream                NVARCHAR(100) NULL,
                    Year                  NVARCHAR(50)  NULL,
                    LecturesInSemester    INT           NOT NULL,
                    LectureTime           NVARCHAR(100) NOT NULL,
                    HasPractical          BIT           NOT NULL DEFAULT 0,
                    PracticalSubjectName  NVARCHAR(100) NULL,
                    PracticalSubjectCode  NVARCHAR(50)  NULL,
                    PracticalsInSemester  INT           NULL,
                    PracticalTiming       NVARCHAR(100) NULL,
                    CreatedOn             DATETIME      NOT NULL DEFAULT GETDATE()
                );
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name='Department' AND Object_ID=Object_ID('Subjects'))
                ALTER TABLE Subjects ADD Department NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name='Stream' AND Object_ID=Object_ID('Subjects'))
                ALTER TABLE Subjects ADD Stream NVARCHAR(100) NULL;
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name='Year' AND Object_ID=Object_ID('Subjects'))
                ALTER TABLE Subjects ADD Year NVARCHAR(50) NULL;");

            // ── Seed Standard College Programmes & Subjects ───────────────
            context.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM Subjects)
                BEGIN
                    -- B.A.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, CreatedOn) VALUES
                    ('English', 'BA-ENG', 'B.A. - English', 45, '09:00 AM - 10:00 AM', 0, GETDATE()),
                    ('Marathi', 'BA-MAR', 'B.A. - Marathi', 45, '10:00 AM - 11:00 AM', 0, GETDATE()),
                    ('Hindi', 'BA-HIN', 'B.A. - Hindi', 45, '11:00 AM - 12:00 PM', 0, GETDATE()),
                    ('History', 'BA-HIS', 'B.A. - History', 45, '12:00 PM - 01:00 PM', 0, GETDATE()),
                    ('Political Science', 'BA-POL', 'B.A. - Political Science', 45, '01:30 PM - 02:30 PM', 0, GETDATE()),
                    ('Economics', 'BA-ECO', 'B.A. - Economics', 45, '02:30 PM - 03:30 PM', 0, GETDATE()),
                    ('Psychology', 'BA-PSY', 'B.A. - Psychology', 45, '09:30 AM - 10:30 AM', 0, GETDATE()),
                    ('Geography', 'BA-GEO', 'B.A. - Geography', 45, '10:30 AM - 11:30 AM', 0, GETDATE()),
                    ('Sociology', 'BA-SOC', 'B.A. - Sociology', 45, '11:30 AM - 12:30 PM', 0, GETDATE()),
                    ('Defence and Strategic Studies', 'BA-DEF', 'B.A. - Defence and Strategic Studies', 45, '01:00 PM - 02:00 PM', 0, GETDATE()),
                    ('Philosophy', 'BA-PHI', 'B.A. - Philosophy', 45, '02:00 PM - 03:00 PM', 0, GETDATE()),
                    ('Logic', 'BA-LOG', 'B.A. - Logic', 45, '03:00 PM - 04:00 PM', 0, GETDATE()),
                    ('Yoga', 'BA-YOG', 'B.A. - Yoga', 45, '08:00 AM - 09:00 AM', 0, GETDATE()),
                    ('Sanskrit', 'BA-SAN', 'B.A. - Sanskrit', 45, '09:00 AM - 10:00 AM', 0, GETDATE());

                    -- M.A.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, CreatedOn) VALUES
                    ('Advanced English Literature', 'MA-ENG', 'M.A. - English', 50, '10:00 AM - 11:00 AM', 0, GETDATE()),
                    ('Advanced Marathi', 'MA-MAR', 'M.A. - Marathi', 50, '11:00 AM - 12:00 PM', 0, GETDATE()),
                    ('Advanced Hindi', 'MA-HIN', 'M.A. - Hindi', 50, '12:00 PM - 01:00 PM', 0, GETDATE()),
                    ('Ancient & Modern History', 'MA-HIS', 'M.A. - History', 50, '01:30 PM - 02:30 PM', 0, GETDATE()),
                    ('Modern Political Theory', 'MA-POL', 'M.A. - Political Science', 50, '02:30 PM - 03:30 PM', 0, GETDATE()),
                    ('Macro & Micro Economics', 'MA-ECO', 'M.A. - Economics', 50, '03:30 PM - 04:30 PM', 0, GETDATE()),
                    ('Applied Psychology', 'MA-PSY', 'M.A. - Psychology', 50, '10:30 AM - 11:30 AM', 0, GETDATE()),
                    ('Advanced Physical Geography', 'MA-GEO', 'M.A. - Geography', 50, '11:30 AM - 12:30 PM', 0, GETDATE());

                    -- B.Com.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, CreatedOn) VALUES
                    ('Accountancy & Taxation', 'BCOM-ACT', 'B.Com. - Accountancy & Taxation', 45, '09:00 AM - 10:00 AM', 0, GETDATE()),
                    ('Marketing Management', 'BCOM-MKT', 'B.Com. - Marketing', 45, '10:00 AM - 11:00 AM', 0, GETDATE()),
                    ('Cost & Management Accounting', 'BCOM-CMA', 'B.Com. - Cost & Management Accounting', 45, '11:00 AM - 12:00 PM', 0, GETDATE()),
                    ('Business Administration', 'BCOM-BA', 'B.Com. - Business Administration', 45, '12:00 PM - 01:00 PM', 0, GETDATE()),
                    ('Business Statistics', 'BCOM-BST', 'B.Com. - Business Statistics', 45, '01:30 PM - 02:30 PM', 0, GETDATE()),
                    ('Banking & Finance', 'BCOM-BNK', 'B.Com. - Banking & Finance', 45, '02:30 PM - 03:30 PM', 0, GETDATE());

                    -- M.Com.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, CreatedOn) VALUES
                    ('Corporate Accountancy & Tax Planning', 'MCOM-ACT', 'M.Com. - Accountancy & Taxation', 50, '10:00 AM - 11:00 AM', 0, GETDATE()),
                    ('Strategic Marketing', 'MCOM-MKT', 'M.Com. - Marketing', 50, '11:00 AM - 12:00 PM', 0, GETDATE()),
                    ('Advanced Cost Accounting', 'MCOM-CMA', 'M.Com. - Cost & Management Accounting', 50, '12:00 PM - 01:00 PM', 0, GETDATE()),
                    ('Corporate Governance & Business Admin', 'MCOM-BA', 'M.Com. - Business Administration', 50, '01:30 PM - 02:30 PM', 0, GETDATE());

                    -- B.Sc.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, PracticalSubjectName, PracticalSubjectCode, PracticalsInSemester, PracticalTiming, CreatedOn) VALUES
                    ('Calculus & Differential Equations', 'BSC-MATH', 'B.Sc. - Mathematics', 45, '09:00 AM - 10:00 AM', 0, NULL, NULL, NULL, NULL, GETDATE()),
                    ('Classical & Modern Physics', 'BSC-PHY', 'B.Sc. - Physics', 45, '10:00 AM - 11:00 AM', 1, 'Physics Lab', 'BSC-PHY-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Plant Anatomy & Physiology', 'BSC-BOT', 'B.Sc. - Botany', 45, '11:00 AM - 12:00 PM', 1, 'Botany Lab', 'BSC-BOT-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Animal Biology & Genetics', 'BSC-ZOO', 'B.Sc. - Zoology', 45, '12:00 PM - 01:00 PM', 1, 'Zoology Lab', 'BSC-ZOO-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Applied Statistics & Probability', 'BSC-STAT', 'B.Sc. - Statistics', 45, '01:30 PM - 02:30 PM', 1, 'Statistics Lab', 'BSC-STAT-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Semiconductors & Circuits', 'BSC-ELE', 'B.Sc. - Electronics', 45, '02:30 PM - 03:30 PM', 1, 'Electronics Lab', 'BSC-ELE-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Microbial Ecology & Genetics', 'BSC-MIC', 'B.Sc. - Microbiology', 45, '09:30 AM - 10:30 AM', 1, 'Microbiology Lab', 'BSC-MIC-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Inorganic & Physical Chemistry', 'BSC-CHEM', 'B.Sc. - Chemistry', 45, '10:30 AM - 11:30 AM', 1, 'Chemistry Lab', 'BSC-CHEM-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Data Structures & Algorithms', 'BSC-CS', 'B.Sc. - Computer Science', 45, '11:30 AM - 12:30 PM', 1, 'Data Structures Lab', 'BSC-CS-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Environmental Science & Ecology', 'BSC-ENV', 'B.Sc. - Environmental Science', 45, '01:00 PM - 02:00 PM', 0, NULL, NULL, NULL, NULL, GETDATE()),
                    ('Food Chemistry & Preservation', 'BSC-FT', 'B.Sc. - Food Technology', 45, '02:00 PM - 03:00 PM', 1, 'Food Tech Lab', 'BSC-FT-P', 15, '02:00 PM - 04:00 PM', GETDATE());

                    -- M.Sc.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, PracticalSubjectName, PracticalSubjectCode, PracticalsInSemester, PracticalTiming, CreatedOn) VALUES
                    ('Topology & Measure Theory', 'MSC-MATH', 'M.Sc. - Mathematics', 50, '09:00 AM - 10:00 AM', 0, NULL, NULL, NULL, NULL, GETDATE()),
                    ('Quantum Mechanics & Electrodynamics', 'MSC-PHY', 'M.Sc. - Physics', 50, '10:00 AM - 11:00 AM', 1, 'Advanced Physics Lab', 'MSC-PHY-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Molecular Biology & Tissue Culture', 'MSC-BOT', 'M.Sc. - Botany', 50, '11:00 AM - 12:00 PM', 1, 'Advanced Botany Lab', 'MSC-BOT-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Applied Zoology & Immunology', 'MSC-ZOO', 'M.Sc. - Zoology', 50, '12:00 PM - 01:00 PM', 1, 'Advanced Zoology Lab', 'MSC-ZOO-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Multivariate Analysis & Inference', 'MSC-STAT', 'M.Sc. - Statistics', 50, '01:30 PM - 02:30 PM', 1, 'Statistical Computing Lab', 'MSC-STAT-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Embedded Systems & VLSI', 'MSC-ELE', 'M.Sc. - Electronics', 50, '02:30 PM - 03:30 PM', 1, 'VLSI Lab', 'MSC-ELE-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Industrial Microbiology & Fermentation', 'MSC-MIC', 'M.Sc. - Microbiology', 50, '09:30 AM - 10:30 AM', 1, 'Fermentation Lab', 'MSC-MIC-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Advanced Organic Chemistry', 'MSC-ORG', 'M.Sc. - Organic Chemistry', 50, '10:30 AM - 11:30 AM', 1, 'Organic Synthesis Lab', 'MSC-ORG-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Inorganic Coordination Compounds', 'MSC-INO', 'M.Sc. - Inorganic Chemistry', 50, '11:30 AM - 12:30 PM', 1, 'Inorganic Lab', 'MSC-INO-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Spectroscopy & Chromatography', 'MSC-ANA', 'M.Sc. - Analytical Chemistry', 50, '01:00 PM - 02:00 PM', 1, 'Analytical Lab', 'MSC-ANA-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Advanced Operating Systems & Networks', 'MSC-CS', 'M.Sc. - Computer Science', 50, '02:00 PM - 03:00 PM', 1, 'Systems Programming Lab', 'MSC-CS-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Machine Learning & Big Data', 'MSC-DS', 'M.Sc. - Data Science', 50, '03:00 PM - 04:00 PM', 1, 'Data Science Lab', 'MSC-DS-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Food Quality & Safety Management', 'MSC-FT', 'M.Sc. - Food Technology', 50, '04:00 PM - 05:00 PM', 1, 'Food Analysis Lab', 'MSC-FT-P', 16, '02:00 PM - 05:00 PM', GETDATE());

                    -- B.Voc.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, PracticalSubjectName, PracticalSubjectCode, PracticalsInSemester, PracticalTiming, CreatedOn) VALUES
                    ('Food Processing & Post Harvest Technology', 'BVOC-FP', 'B.Voc. - Food Processing & Post Harvest Technology', 45, '09:00 AM - 10:00 AM', 1, 'Harvest Processing Lab', 'BVOC-FP-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Journalism & Mass Communication', 'BVOC-JMC', 'B.Voc. - Journalism & Mass Communication', 45, '10:00 AM - 11:00 AM', 1, 'Media Production Lab', 'BVOC-JMC-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Retail Management', 'BVOC-RM', 'B.Voc. - Retail Management', 45, '11:00 AM - 12:00 PM', 0, NULL, NULL, NULL, NULL, GETDATE()),
                    ('Dairy Technology', 'BVOC-DT', 'B.Voc. - Dairy Technology', 45, '12:00 PM - 01:00 PM', 1, 'Dairy Processing Lab', 'BVOC-DT-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('E-Commerce & Digital Marketing', 'BVOC-DM', 'B.Voc. - E-Commerce & Digital Marketing', 45, '01:30 PM - 02:30 PM', 1, 'Digital Marketing Lab', 'BVOC-DM-P', 15, '02:00 PM - 04:00 PM', GETDATE());

                    -- M.Voc.
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, PracticalSubjectName, PracticalSubjectCode, PracticalsInSemester, PracticalTiming, CreatedOn) VALUES
                    ('Advanced Food Processing Technology', 'MVOC-FPT', 'M.Voc. - Food Processing Technology', 50, '10:00 AM - 11:00 AM', 1, 'Advanced Food Tech Lab', 'MVOC-FPT-P', 16, '02:00 PM - 05:00 PM', GETDATE()),
                    ('Advanced Media Studies & Production', 'MVOC-MS', 'M.Voc. - Media Studies', 50, '11:00 AM - 12:00 PM', 1, 'Broadcast Studio Lab', 'MVOC-MS-P', 16, '02:00 PM - 05:00 PM', GETDATE());

                    -- Professional
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, PracticalSubjectName, PracticalSubjectCode, PracticalsInSemester, PracticalTiming, CreatedOn) VALUES
                    ('Principles of Business Management', 'BBA-GEN', 'Professional - Bachelor of Business Administration', 45, '09:00 AM - 10:00 AM', 0, NULL, NULL, NULL, NULL, GETDATE()),
                    ('Database Management Systems (CA)', 'BBA-CA', 'Professional - Bachelor of Business Administration (Computer Applications)', 45, '10:00 AM - 11:00 AM', 1, 'DBMS Lab', 'BBA-CA-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Object Oriented Programming (BCA)', 'BCA-GEN', 'Professional - Bachelor of Computer Applications', 45, '11:00 AM - 12:00 PM', 1, 'OOP Lab', 'BCA-P', 15, '02:00 PM - 04:00 PM', GETDATE());

                    -- Diploma
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, CreatedOn) VALUES
                    ('Direct & Indirect Taxation Laws', 'DIP-TAX', 'Diploma - Post graduate Diploma in Taxation Laws', 40, '09:00 AM - 10:00 AM', 0, GETDATE());

                    -- Ph.D. Coursework
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, CreatedOn) VALUES
                    ('Research Methodology (Botany)', 'PHD-BOT', 'Ph.D. - Botany', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Physics)', 'PHD-PHY', 'Ph.D. - Physics', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Microbiology)', 'PHD-MIC', 'Ph.D. - Microbiology', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Economics)', 'PHD-ECO', 'Ph.D. - Economics', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Marathi)', 'PHD-MAR', 'Ph.D. - Marathi', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (English)', 'PHD-ENG', 'Ph.D. - English', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Hindi)', 'PHD-HIN', 'Ph.D. - Hindi', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Statistics)', 'PHD-STAT', 'Ph.D. - Statistics', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Chemistry)', 'PHD-CHEM', 'Ph.D. - Chemistry', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Geography)', 'PHD-GEO', 'Ph.D. - Geography', 30, '03:00 PM - 04:30 PM', 0, GETDATE()),
                    ('Research Methodology (Commerce & Mgmt)', 'PHD-COM', 'Ph.D. - Commerce & Management', 30, '03:00 PM - 04:30 PM', 0, GETDATE());

                    -- Others
                    INSERT INTO Subjects (SubjectName, SubjectCode, Department, LecturesInSemester, LectureTime, HasPractical, PracticalSubjectName, PracticalSubjectCode, PracticalsInSemester, PracticalTiming, CreatedOn) VALUES
                    ('Library Information Systems', 'BLIS-GEN', 'Others - Bachelor of Library & Information Science', 40, '10:00 AM - 11:00 AM', 1, 'Library Automation Lab', 'BLIS-P', 15, '02:00 PM - 04:00 PM', GETDATE()),
                    ('Digital Information Management', 'MLIS-GEN', 'Others - Master of Library & Information Science', 40, '11:00 AM - 12:00 PM', 1, 'Digital Archives Lab', 'MLIS-P', 15, '02:00 PM - 04:00 PM', GETDATE());
                END");
        }
    }
}
