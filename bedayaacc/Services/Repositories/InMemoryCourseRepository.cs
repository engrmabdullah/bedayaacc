using bedayaacc.Models;

namespace bedayaacc.Services.Repositories
{
    public class InMemoryCourseRepository : ICourseRepository
    {
        private readonly List<CourseDetailsDto> _data = new();

        public InMemoryCourseRepository()
        {
            // Seed تجريبي (تفاصيل لكل كورس)
            _data.Clear();
            _data.AddRange(new[]
            {
  new CourseDetailsDto{
    Id=1, TitleAr="أساسيات المحاسبة", TitleEn="Accounting Basics",
    Level=CourseLevel.Beginner, DurationMinutes=180, Price=0,
    SummaryAr="مدخل عملي للمحاسبة المالية مع أمثلة وحالات تطبيقية.",
    SummaryEn="Practical intro to financial accounting with examples.",
    InstructorAr="أ. أحمد السعيد", InstructorEn="Ahmed El-Saeed",
    Track = new CourseTrack{
        Slug="foundation",
        TitleAr="مسار التأسيس المحاسبي",
        TitleEn="Accounting Foundation Track",
        DescriptionAr="يمهّد لفهم القوائم والإطار المحاسبي قبل التخصص.",
        DescriptionEn="Lays the groundwork before specialization."
    },
    OutcomesAr = { "فهم الإطار المفاهيمي للمحاسبة", "قراءة القوائم المالية الأساسية", "تمييز أنواع الحسابات" },
    OutcomesEn = { "Understand accounting framework", "Read basic financial statements", "Differentiate account types" },
    RequirementsAr = { "لا توجد متطلبات — مناسب للمبتدئين" },
    RequirementsEn = { "No prerequisites — beginner friendly" },
    AudienceAr = { "طلبة تجارة", "حديثو التخرج", "غير المحاسبين الراغبين في التحول" },
    AudienceEn = { "Business students", "Fresh grads", "Career shifters" },
    Sections = new(){ /* كما هي عندك */ },
    Faq = new(){ /* كما هي عندك */ },
    CreatedAt=DateTime.UtcNow.AddDays(-2),
    Tags = new(){ "accounting", "foundation" },
},


    new CourseDetailsDto{
        Id=2, TitleAr="محاسبة التكاليف", TitleEn="Cost Accounting",
        Level=CourseLevel.Intermediate, DurationMinutes=240, Price=299,
        SummaryAr="تخطيط التكاليف وتحليل الانحرافات ونقاط التعادل.",
        SummaryEn="Cost planning, variance analysis, and break-even.",
        InstructorAr="د. منى فؤاد", InstructorEn="Mona Fouad",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="مفاهيم أساسية", TitleEn="Essential Concepts",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="أنواع التكاليف", TitleEn="Types of Costs", DurationMinutes=20 },
                               new CourseCurriculumItem{ Order=2, TitleAr="تحليل CVP", TitleEn="CVP Analysis", DurationMinutes=30 } } },
            new CourseSection{ Order=2, TitleAr="الموازنات", TitleEn="Budgets",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="الموازنة المرنة", TitleEn="Flexible Budget", DurationMinutes=25 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل يتطلب خبرة سابقة؟", AnswerAr="يفضل معرفة أساسيات المحاسبة.", QuestionEn="Prerequisites?", AnswerEn="Basic accounting knowledge helps."}},
        CreatedAt=DateTime.UtcNow.AddDays(-5),
    Tags = new(){ "accounting", "foundation" },
    },

   new CourseDetailsDto{
    Id=3, TitleAr="ضرائب القيمة المضافة (السعودية)", TitleEn="VAT Fundamentals (KSA)",
    Level=CourseLevel.Advanced, DurationMinutes=200, Price=199,
    SummaryAr="فهم نظام ضريبة القيمة المضافة في السعودية وتطبيقاته العملية.",
    SummaryEn="Understand KSA VAT regime with practical applications.",
    InstructorAr="أ. سامي الحربي", InstructorEn="Sami Alharbi",
    Track = new CourseTrack{
        Slug="tax-vat",
        TitleAr="مسار الضرائب والامتثال",
        TitleEn="Tax & Compliance Track",
        DescriptionAr="يركز على لوائح الزكاة والضريبة والإقرارات.",
        DescriptionEn="Focuses on tax regulations and filings."
    },
    OutcomesAr = { "تحديد نطاق الخضوع والاعفاءات", "إعداد الإقرارات الدورية", "معالجة الحالات الخاصة" },
    OutcomesEn = { "Determine scope & exemptions", "Prepare periodic returns", "Handle special cases" },
    RequirementsAr = { "أساسيات محاسبة", "إلمام بالمصطلحات الضريبية" },
    RequirementsEn = { "Basic accounting", "Familiarity with tax terms" },
    AudienceAr = { "محاسبون", "أخصائيو ضرائب", "أصحاب الأعمال" },
    AudienceEn = { "Accountants", "Tax specialists", "Business owners" },
    Sections = new(){ /* كما هي عندك */ },
    Faq = new(){ /* كما هي عندك */ },
    CreatedAt=DateTime.UtcNow.AddDays(-1),
Tags = new(){ "محاسبة", "أساسيات", "قوائم مالية", "Beginners", "Accounting" },
},


    new CourseDetailsDto{
        Id=4, TitleAr="IFRS للمبتدئين", TitleEn="IFRS for Beginners",
        Level=CourseLevel.Beginner, DurationMinutes=180, Price=249,
        SummaryAr="التعريف بالمعايير الدولية لإعداد التقارير المالية وأهم الفروقات.",
        SummaryEn="Intro to IFRS and key differences.",
        InstructorAr="د. خالد عوض", InstructorEn="Khaled Awad",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="الإطار العام", TitleEn="Framework",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="مفهوم IFRS", TitleEn="What is IFRS", DurationMinutes=20 } } },
            new CourseSection{ Order=2, TitleAr="معايير مختارة", TitleEn="Selected Standards",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="IAS 1", TitleEn="IAS 1", DurationMinutes=25 },
                               new CourseCurriculumItem{ Order=2, TitleAr="IFRS 15", TitleEn="IFRS 15", DurationMinutes=30 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل يناسب حديثي التخرج؟", AnswerAr="نعم.", QuestionEn="For fresh grads?", AnswerEn="Yes."}},
        CreatedAt=DateTime.UtcNow.AddDays(-8),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=5, TitleAr="Excel للمحاسبين", TitleEn="Excel for Accountants",
        Level=CourseLevel.Beginner, DurationMinutes=150, Price=0,
        SummaryAr="تمارين عملية على الدوال المحاسبية والجداول المحورية.",
        SummaryEn="Hands-on with accounting functions and pivot tables.",
        InstructorAr="أ. رنا عبدالكريم", InstructorEn="Rana Abdelkarim",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="الدوال", TitleEn="Functions",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="SUMIF / SUMIFS", TitleEn="SUMIF / SUMIFS", DurationMinutes=20 },
                               new CourseCurriculumItem{ Order=2, TitleAr="VLOOKUP/XLOOKUP", TitleEn="VLOOKUP/XLOOKUP", DurationMinutes=25 } } },
            new CourseSection{ Order=2, TitleAr="Pivot", TitleEn="Pivot",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="Pivot Basics", TitleEn="Pivot Basics", DurationMinutes=20 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل نحتاج نسخة Excel حديثة؟", AnswerAr="يفضل 2019 أو 365.", QuestionEn="Excel version?", AnswerEn="Prefer 2019/365."}},
        CreatedAt=DateTime.UtcNow.AddDays(-10),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=6, TitleAr="المراجعة الداخلية العملية", TitleEn="Practical Internal Auditing",
        Level=CourseLevel.Advanced, DurationMinutes=300, Price=399,
        SummaryAr="منهجية المراجعة الداخلية وخطط العمل وتقارير الملاحظات.",
        SummaryEn="Internal audit methodology, workplans, and reporting.",
        InstructorAr="م. علي بخيت", InstructorEn="Ali Bakhit",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="المنهجية", TitleEn="Methodology",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="تقييم المخاطر", TitleEn="Risk Assessment", DurationMinutes=30 },
                               new CourseCurriculumItem{ Order=2, TitleAr="خطة المراجعة", TitleEn="Audit Plan", DurationMinutes=35 } } },
            new CourseSection{ Order=2, TitleAr="التقارير", TitleEn="Reporting",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="كتابة الملاحظات", TitleEn="Writing Findings", DurationMinutes=25 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل يشمل قوالب عملية؟", AnswerAr="نعم تحميل قوالب.", QuestionEn="Templates included?", AnswerEn="Yes."}},
        CreatedAt=DateTime.UtcNow.AddDays(-7),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=7, TitleAr="تحليل القوائم المالية", TitleEn="Financial Statement Analysis",
        Level=CourseLevel.Intermediate, DurationMinutes=210, Price=249,
        SummaryAr="نسب مالية، تحليل الاتجاهات، والتقييم المبدئي.",
        SummaryEn="Ratios, trend analysis, and basic valuation.",
        InstructorAr="أ. ماهر حجازي", InstructorEn="Maher Hijazi",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="النسب", TitleEn="Ratios",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="السيولة والربحية", TitleEn="Liquidity & Profitability", DurationMinutes=25 } } },
            new CourseSection{ Order=2, TitleAr="الاتجاهات", TitleEn="Trends",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="تحليل أفقي", TitleEn="Horizontal Analysis", DurationMinutes=20 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="ملفات عمل؟", AnswerAr="نعم Excel جاهز.", QuestionEn="Working files?", AnswerEn="Yes, Excel."}},
        CreatedAt=DateTime.UtcNow.AddDays(-3),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=8, TitleAr="إعداد الرواتب والزكاة", TitleEn="Payroll & Zakat Basics",
        Level=CourseLevel.Intermediate, DurationMinutes=180, Price=199,
        SummaryAr="حساب الرواتب والاستقطاعات ومبادئ الزكاة للشركات.",
        SummaryEn="Payroll calculations, deductions, and corporate zakat basics.",
        InstructorAr="أ. وجيه الجهني", InstructorEn="Wajeeh Al-Juhani",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="الرواتب", TitleEn="Payroll",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="الاستقطاعات", TitleEn="Deductions", DurationMinutes=20 } } },
            new CourseSection{ Order=2, TitleAr="الزكاة", TitleEn="Zakat",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="أساسيات الحساب", TitleEn="Calculation Basics", DurationMinutes=25 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="نطاق الزكاة؟", AnswerAr="شرحنا الحالات الشائعة.", QuestionEn="Scope?", AnswerEn="Covers common cases."}},
        CreatedAt=DateTime.UtcNow.AddDays(-12),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=9, TitleAr="Power BI للمحاسبين", TitleEn="Power BI for Accountants",
        Level=CourseLevel.Intermediate, DurationMinutes=240, Price=349,
        SummaryAr="نظرة كاملة على Model وDAX وتقارير مالية تفاعلية.",
        SummaryEn="Modeling, DAX, and interactive financial reports.",
        InstructorAr="م. هند صادق", InstructorEn="Hind Sadiq",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="البيانات", TitleEn="Data",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="النمذجة", TitleEn="Modeling", DurationMinutes=30 } } },
            new CourseSection{ Order=2, TitleAr="DAX", TitleEn="DAX",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="Measures الأساسية", TitleEn="Basic Measures", DurationMinutes=30 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل نستخدم ملفات محاسبية فعلية؟", AnswerAr="نعم.", QuestionEn="Real accounting data?", AnswerEn="Yes."}},
        CreatedAt=DateTime.UtcNow.AddDays(-9),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=10, TitleAr="إدارة المخزون والتكلفة", TitleEn="Inventory & Costing",
        Level=CourseLevel.Intermediate, DurationMinutes=210, Price=279,
        SummaryAr="طرق التسعير (FIFO/LIFO/AVG) والقيود المحاسبية.",
        SummaryEn="Costing methods and accounting entries.",
        InstructorAr="أ. لمياء اليوسف", InstructorEn="Lamia AlYousef",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="الطرق", TitleEn="Methods",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="FIFO & AVG", TitleEn="FIFO & AVG", DurationMinutes=25 } } },
            new CourseSection{ Order=2, TitleAr="القيود", TitleEn="Entries",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="إثبات الهالك", TitleEn="Write-offs", DurationMinutes=20 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="دراسة حالة؟", AnswerAr="نعم حالة متجر تجزئة.", QuestionEn="Case study?", AnswerEn="Retail case."}},
        CreatedAt=DateTime.UtcNow.AddDays(-11),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=11, TitleAr="QuickBooks للمبتدئين", TitleEn="QuickBooks for Beginners",
        Level=CourseLevel.Beginner, DurationMinutes=160, Price=199,
        SummaryAr="التأسيس والترحيل وبناء دليل الحسابات وإصدار الفواتير.",
        SummaryEn="Setup, chart of accounts, and invoicing.",
        InstructorAr="أ. رائد سلامة", InstructorEn="Raed Salama",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="التهيئة", TitleEn="Setup",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="دليل الحسابات", TitleEn="Chart of Accounts", DurationMinutes=20 } } },
            new CourseSection{ Order=2, TitleAr="المعاملات", TitleEn="Transactions",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="مبيعات ومشتريات", TitleEn="Sales & Purchases", DurationMinutes=30 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="نسخة البرنامج؟", AnswerAr="نستعمل النسخة الأونلاين.", QuestionEn="Which edition?", AnswerEn="Online edition."}},
        CreatedAt=DateTime.UtcNow.AddDays(-6),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=12, TitleAr="Odoo Accounting عملي", TitleEn="Odoo Accounting Hands-on",
        Level=CourseLevel.Intermediate, DurationMinutes=240, Price=399,
        SummaryAr="تهيئة الحسابات، الفواتير، المصروفات، والتقارير في Odoo.",
        SummaryEn="Setup accounts, invoicing, expenses, and reports in Odoo.",
        InstructorAr="م. نجلاء عامر", InstructorEn="Nagla Amer",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="الإعداد", TitleEn="Config",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="الشجرة المحاسبية", TitleEn="COA", DurationMinutes=25 } } },
            new CourseSection{ Order=2, TitleAr="التطبيق", TitleEn="Apply",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="الفواتير والمدفوعات", TitleEn="Invoices & Payments", DurationMinutes=35 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل نستخدم ديمو؟", AnswerAr="نعم قاعدة تجريبية.", QuestionEn="Demo DB?", AnswerEn="Yes."}},
        CreatedAt=DateTime.UtcNow.AddDays(-14),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=13, TitleAr="CMA Basics تمهيدي", TitleEn="CMA Basics Prep",
        Level=CourseLevel.Beginner, DurationMinutes=200, Price=299,
        SummaryAr="تمهيد لمفاهيم الـCMA الأساسية وبناء خطة مذاكرة.",
        SummaryEn="Intro concepts for CMA and study planning.",
        InstructorAr="أ. شيرين محروس", InstructorEn="Sherin Mahrous",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="Part 1 نظرة عامة", TitleEn="Part 1 Overview",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="External Financial Reporting", TitleEn="External Financial Reporting", DurationMinutes=25 } } },
            new CourseSection{ Order=2, TitleAr="Part 2 نظرة عامة", TitleEn="Part 2 Overview",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="Corporate Finance", TitleEn="Corporate Finance", DurationMinutes=25 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل يشمل بنك أسئلة؟", AnswerAr="روابط لمصادر مجانية.", QuestionEn="Question bank?", AnswerEn="Links to free sources."}},
        CreatedAt=DateTime.UtcNow.AddDays(-4),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=14, TitleAr="CPA Basics تمهيدي", TitleEn="CPA Basics Prep",
        Level=CourseLevel.Beginner, DurationMinutes=210, Price=299,
        SummaryAr="تعريف بمحتوى CPA وأهم المواضيع وحيلة المذاكرة.",
        SummaryEn="CPA content overview and study hacks.",
        InstructorAr="أ. طارق درويش", InstructorEn="Tarek Darwish",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="AUD/REG/FA/ISC", TitleEn="AUD/REG/FA/ISC",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="نظرة عامة", TitleEn="High-level Overview", DurationMinutes=30 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="يناسب مين؟", AnswerAr="للمبتدئين تمام.", QuestionEn="Who is it for?", AnswerEn="Beginners."}},
        CreatedAt=DateTime.UtcNow.AddDays(-13),
    Tags = new(){ "accounting", "foundation" },
    },

    new CourseDetailsDto{
        Id=15, TitleAr="التحكم الداخلي والامتثال", TitleEn="Internal Control & Compliance",
        Level=CourseLevel.Advanced, DurationMinutes=260, Price=379,
        SummaryAr="تصميم ضوابط داخلية فعّالة وإدارة المخاطر والامتثال.",
        SummaryEn="Design effective internal controls, risk, and compliance.",
        InstructorAr="أ. ولاء الطاهر", InstructorEn="Walaa Altaher",
        Sections=new(){
            new CourseSection{ Order=1, TitleAr="إطار COSO", TitleEn="COSO Framework",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="المكونات الخمسة", TitleEn="Five Components", DurationMinutes=30 } } },
            new CourseSection{ Order=2, TitleAr="التطبيق العملي", TitleEn="Practical Application",
                Lessons=new(){ new CourseCurriculumItem{ Order=1, TitleAr="ضوابط دورة المشتريات", TitleEn="P2P Controls", DurationMinutes=35 } } }
        },
        Faq=new(){ new CourseFaq{ Order=1, QuestionAr="هل في Checklists؟", AnswerAr="نعم ملفات PDF.", QuestionEn="Checklists?", AnswerEn="Yes, PDFs."}},
        CreatedAt=DateTime.UtcNow.AddDays(-15),
    Tags = new(){ "accounting", "foundation" },
    }
});

        }

        public Task<IReadOnlyList<CourseDto>> GetLatestAsync(int take, string lang)
        {
            var items = _data.Where(x => x.IsPublished)
                             .OrderByDescending(x => x.CreatedAt)
                             .Take(take)
                             .Select(x => (CourseDto)x)
                             .ToList()
                             .AsReadOnly();
            return Task.FromResult((IReadOnlyList<CourseDto>)items);
        }


        public Task<PagedResult<CourseDto>> GetAsync(CourseFilter filter, string lang)
        {
            var q = _data.Where(x => x.IsPublished).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.TrackSlug))
                q = q.Where(x => x.Track.Slug == filter.TrackSlug);

            q = ApplyCommonFilters(q, filter);

            var total = q.Count();
            var items = q.OrderByDescending(x => x.CreatedAt)
                         .Skip((filter.Page - 1) * filter.PageSize)
                         .Take(filter.PageSize)
                         .Select(x => (CourseDto)x)
                         .ToList();

            return Task.FromResult(new PagedResult<CourseDto>
            {
                Items = items,

                Page = filter.Page,
                PageSize = filter.PageSize
            });
        }

        public Task<CourseDetailsDto?> GetByIdAsync(int id, string lang)
        {
            var item = _data.FirstOrDefault(x => x.Id == id && x.IsPublished);
            return Task.FromResult(item);
        }

        public async Task<HashSet<string>> GetAllTagsAsync()
        {
            var tags = _data.Where(x => x.IsPublished)
                            .SelectMany(x => x.Tags)
                            .Where(t => !string.IsNullOrWhiteSpace(t))
                            .Select(t => t.Trim().ToLowerInvariant())
                            .ToHashSet();
            return await Task.FromResult(tags);
        }

        public async Task<IReadOnlyList<CourseDto>> GetRelatedAsync(int courseId, int take, string lang)
        {
            var me = _data.FirstOrDefault(x => x.Id == courseId);
            if (me is null) return Array.Empty<CourseDto>();

            // تشابه بالوسوم + نفس المسار (إن وجد)
            var meTags = me.Tags.Select(t => t.ToLowerInvariant()).ToHashSet();
            var q = _data.Where(x => x.Id != courseId && x.IsPublished);

            if (me.Track is not null)
                q = q.Where(x => x.Track?.Slug == me.Track.Slug);

            q = q.OrderByDescending(x => x.Tags.Count(t => meTags.Contains(t.ToLowerInvariant())))
                 .ThenByDescending(x => x.CreatedAt);

            var items = q.Take(take).Select(x => (CourseDto)x).ToList().AsReadOnly();
            return await Task.FromResult(items);
        }

        public async Task<IReadOnlyList<CourseTrack>> GetTracksAsync(string lang)
        {
            var tracks = _data.Where(x => x.Track is not null)
                              .Select(x => x.Track!)
                              .GroupBy(t => t.Slug)
                              .Select(g => g.First())
                              .ToList()
                              .AsReadOnly();
            return await Task.FromResult(tracks);
        }

        public async Task<PagedResult<CourseDto>> GetByTrackAsync(string trackSlug, CourseFilter filter, string lang)
        {
            var q = _data.Where(x => x.IsPublished && x.Track?.Slug == trackSlug).AsQueryable();

            // إعادة استخدام نفس منطق الفلترة (بحث/مستوى/مجاني/وسم)
            q = ApplyCommonFilters(q, filter);

            var total = q.Count();
            var items = q.OrderByDescending(x => x.CreatedAt)
                         .Skip((filter.Page - 1) * filter.PageSize)
                         .Take(filter.PageSize)
                         .Select(x => (CourseDto)x)
                         .ToList();

            return await Task.FromResult(new PagedResult<CourseDto>
            {
                Items = items,

                Page = filter.Page,
                PageSize = filter.PageSize
            });
        }

        // فلترة عامة مشتركة (Helper داخلي)
        private static IQueryable<CourseDetailsDto> ApplyCommonFilters(IQueryable<CourseDetailsDto> q, CourseFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                var s = filter.Query.Trim();
                q = q.Where(x => x.TitleAr.Contains(s, StringComparison.OrdinalIgnoreCase)
                              || x.TitleEn.Contains(s, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.Level.HasValue)
                q = q.Where(x => x.Level == filter.Level.Value);

            if (filter.FreeOnly.HasValue)
                q = filter.FreeOnly.Value ? q.Where(x => x.IsFree) : q.Where(x => !x.IsFree);

            // فلترة بالوسم (لو أضفتها للفلتر)
            if (!string.IsNullOrWhiteSpace(filter.Tag))
            {
                var tag = filter.Tag.Trim().ToLowerInvariant();
                q = q.Where(x => x.Tags.Any(t => t.ToLowerInvariant() == tag));
            }

            return q;
        }

    }
}
