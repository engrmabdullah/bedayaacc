namespace bedayaacc.Models
{
    public enum CourseLevel { Beginner, Intermediate, Advanced }

    public class CourseDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public CourseLevel Level { get; set; }
        public int DurationMinutes { get; set; }   // المدة بالدقائق
        public decimal Price { get; set; }         // 0 => مجاني
        public string ThumbnailUrl { get; set; } = "https://ibsacademy.org/U/c/c-img-338.jpg";
        public bool IsPublished { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Title(string lang) => lang == "ar" ? TitleAr : TitleEn;
        public bool IsFree => Price <= 0;

        public List<string> Tags { get; set; } = new();   // ["excel","ifrs","vat","bi",...]
        public string? TrackSlug => (this is CourseDetailsDto d && d.Track != null) ? d.Track.Slug : null;

    }

    public class CourseFilter
    {
        public string? Query { get; set; }
        public CourseLevel? Level { get; set; }
        public bool? FreeOnly { get; set; }   // null => الكل
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;


        public string? Tag { get; set; }          // فلترة بوسم
        public string? TrackSlug { get; set; }    // فلترة بمسار (للاستخدام العام)
    }



    public class CourseCurriculumItem
    {
        public int Order { get; set; }
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public int? DurationMinutes { get; set; } // للحصة
        public string Title(string lang) => lang == "ar" ? TitleAr : TitleEn;
    }

    public class CourseSection
    {
        public int Order { get; set; }
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public List<CourseCurriculumItem> Lessons { get; set; } = new();
        public string Title(string lang) => lang == "ar" ? TitleAr : TitleEn;
    }


    public class CourseFaq
    {
        public int Order { get; set; }
        public string QuestionAr { get; set; } = "";
        public string AnswerAr { get; set; } = "";
        public string QuestionEn { get; set; } = "";
        public string AnswerEn { get; set; } = "";
        public string Q(string lang) => lang == "ar" ? QuestionAr : QuestionEn;
        public string A(string lang) => lang == "ar" ? AnswerAr : AnswerEn;
    }

    public class CourseDetailsDto : CourseDto
    {
        public string SummaryAr { get; set; } = "";
        public string SummaryEn { get; set; } = "";
        public string InstructorAr { get; set; } = "مدرب";
        public string InstructorEn { get; set; } = "Instructor";
        public string RegisterUrl { get; set; } = "/contact"; // عدّلها لاحقًا لو في دفع
        public List<CourseSection> Sections { get; set; } = new();
        public List<CourseFaq> Faq { get; set; } = new();

        public string Summary(string lang) => lang == "ar" ? SummaryAr : SummaryEn;
        public string Instructor(string lang) => lang == "ar" ? InstructorAr : InstructorEn;

        public CourseTrack? Track { get; set; }

        public List<string> OutcomesAr { get; set; } = new();     // ما ستتعلمه
        public List<string> OutcomesEn { get; set; } = new();

        public List<string> RequirementsAr { get; set; } = new(); // المتطلبات
        public List<string> RequirementsEn { get; set; } = new();

        public List<string> AudienceAr { get; set; } = new();     // الجمهور المستهدف
        public List<string> AudienceEn { get; set; } = new();

        public List<string>? Tags { get; set; }      // فلترة حسب التاجز
        public string? TrackSlug { get; set; }       // فلترة حسب المسار


    }

    public class CourseTrack
    {
        public string Slug { get; set; } = "";         // مثلاً "accounting-basics"
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public string Title(string lang) => lang == "ar" ? TitleAr : TitleEn;
        public string? Description(string lang) => lang == "ar" ? DescriptionAr : DescriptionEn;
    }
}
