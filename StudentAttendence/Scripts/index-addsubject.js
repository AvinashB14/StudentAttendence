// ═════════════════════════════════════════════════════════════════
// STREAM_DATA: Stream -> Department -> Subject Hierarchy
// ═════════════════════════════════════════════════════════════════
var STREAM_DATA = {
    "Arts": {
        depts: {
            "English": [
                "English Communication", "British Literature", "Indian English Poetry",
                "Post-Colonial Studies", "Literary Criticism & Theory", "Advanced Linguistics"
            ],
            "Marathi": [
                "Marathi Sahitya", "Madhyayugin Marathi Kavya", "Bhasha Vidnyan",
                "Marathi Natak & Cinema", "Loksahitya", "Vyavaharik Marathi"
            ],
            "Hindi": [
                "Hindi Sahitya Ka Itihas", "Kavya Shastra", "Adhunik Hindi Gadya",
                "Natak & Rangmanch", "Prayojanmulak Hindi", "Samkalin Hindi Kavita"
            ],
            "History": [
                "Ancient Indian History", "Medieval India (Mughals & Marathas)",
                "Modern Indian History & Freedom Struggle", "World History (1914-1992)",
                "Chhatrapati Shivaji Maharaj & His Times", "Historiography & Research Methodology"
            ],
            "Geography": [
                "Physical Geography (Geomorphology)", "Human & Economic Geography",
                "Geography of India & Maharashtra", "Climatology & Oceanography",
                "Cartography & Map Reading", "GIS & Remote Sensing"
            ]
        }
    },
    "Commerce": {
        depts: {
            "Accountancy & Taxation": [
                "Financial Accounting", "Corporate Accounting", "Cost Accounting",
                "Income Tax Law & Practice", "Goods & Services Tax (GST)", "Auditing & Assurance"
            ],
            "Marketing": [
                "Principles of Marketing", "Consumer Behaviour", "Services Marketing",
                "Advertising & Sales Promotion", "Digital & Social Media Marketing", "International Marketing"
            ],
            "Cost & Management Accounting": [
                "Cost Concepts & Classifications", "Methods of Costing",
                "Management Accounting & Control", "Budgetary Control & Standard Costing",
                "Marginal Costing & Decision Making", "Strategic Cost Management"
            ],
            "Banking & Finance": [
                "Banking Law & Practice", "Indian Financial System & RBI",
                "Financial Markets & Instruments", "Credit Management & NPA",
                "Investment Analysis & Portfolio Management", "FinTech & Digital Banking"
            ],
            "Business Statistics": [
                "Descriptive Statistics & Probability", "Business Mathematics & Matrices",
                "Sampling Distributions & Estimation", "Hypothesis Testing & Chi-Square",
                "Correlation & Linear Regression", "Time Series & Index Numbers"
            ]
        }
    },
    "Diploma": {
        depts: {
            "PG Diploma in Taxation Laws": [
                "Direct Taxes & Income Tax Act", "Goods and Services Tax (GST) & Customs",
                "Tax Audit, Procedures & Case Laws", "Book Keeping, Accountancy & Tax Planning"
            ]
        }
    },
    "Science": {
        depts: {
            "Mathematics": [
                "Calculus & Differential Equations", "Linear Algebra & Matrices",
                "Real Analysis & Metric Spaces", "Abstract Algebra (Groups & Rings)",
                "Complex Analysis", "Numerical Methods & Differential Calculus"
            ],
            "Physics": [
                "Mechanics & Properties of Matter", "Electricity, Magnetism & Electrodynamics",
                "Optics & Lasers", "Thermodynamics & Statistical Mechanics",
                "Quantum Mechanics & Atomic Physics", "Nuclear Physics & Electronics"
            ],
            "Botany": [
                "Plant Diversity (Algae, Fungi, Bryophytes)", "Plant Anatomy & Embryology",
                "Plant Physiology & Biochemistry", "Genetics & Plant Breeding",
                "Ecology & Phytogeography", "Economic Botany & Biotechnology"
            ],
            "Zoology": [
                "Animal Diversity (Non-Chordates & Chordates)", "Cell Biology & Genetics",
                "Animal Physiology & Endocrinology", "Evolution & Developmental Biology",
                "Applied Zoology (Apiculture, Sericulture)", "Immunology & Molecular Biology"
            ],
            "Electronics": [
                "Basic Electronics & Semiconductor Devices", "Digital Electronics & Logic Gates",
                "Operational Amplifiers & Linear ICs", "Microprocessors & Microcontrollers (8051/ARM)",
                "Communication Electronics & Signals", "Embedded Systems & IoT"
            ],
            "Computer Science": [
                "Problem Solving & C/C++ Programming", "Data Structures & Algorithms",
                "Object Oriented Programming (Java/Python)", "Database Management Systems (SQL)",
                "Operating Systems & Computer Networks", "Web Development & Software Engineering"
            ],
            "Chemistry": [
                "Physical Chemistry (Kinetics, Thermodynamics)", "Inorganic Chemistry & Coordination Compounds",
                "Organic Chemistry (Reaction Mechanisms, Synthesis)", "Analytical Chemistry & Spectroscopy",
                "Polymer Chemistry & Green Synthesis", "Biochemistry & Industrial Chemistry"
            ],
            "Food Technology": [
                "Food Chemistry & Composition", "Food Microbiology & Safety",
                "Principles of Food Processing & Preservation", "Post-Harvest Technology",
                "Dairy & Beverage Processing", "Food Packaging & Quality Assurance"
            ],
            "Data Science": [
                "Python for Data Science", "Data Wrangling & Visualization",
                "Statistical Foundations & Linear Algebra", "Machine Learning & Predictive Modeling",
                "Big Data Frameworks", "Deep Learning & Neural Networks"
            ]
        }
    },
    "PHD": {
        depts: {
            "Botany": ["Research Methodology in Plant Sciences", "Advanced Botanical Literature"],
            "Economics": ["Advanced Econometric Techniques", "Development Economics Research"],
            "Hindi": ["Hindi Sahitya Shodh Pravidhi", "Bhasha Aur Sahitya Shodh"],
            "Physics": ["Advanced Condensed Matter Physics", "Experimental Physics Research"],
            "Marathi": ["Marathi Bhasha Va Sahitya Shodh", "Vangmayeen Chalwali Shodh"],
            "Statistics": ["Advanced Statistical Inference", "Multivariate Data Modeling"],
            "Commerce & Management": ["Business Research Methods", "Strategic Corporate Research"],
            "Microbiology": ["Applied Molecular Microbiology", "Bioinformatics & Microbial Genomics"]
        }
    },
    "Vocational": {
        depts: {
            "Food Processing & Post Harvest Technology": [
                "Post-Harvest Handling & Storage", "Fruits & Vegetables Processing",
                "Grain & Bakery Technology", "Food Safety Standards & FSSAI"
            ],
            "Journalism & Mass Communication": [
                "Reporting & News Editing", "Electronic & Broadcast Media",
                "Photojournalism & Video Production", "Media Laws & Ethics"
            ],
            "Retail Management": [
                "Retail Store Operations", "Merchandise Buying & Visual Merchandising",
                "Customer Relationship Management (CRM)", "Supply Chain & Logistics"
            ],
            "Dairy Technology": [
                "Milk Processing & Plant Design", "Cheese & Fermented Milk Products",
                "Dairy Microbiology & Quality Control", "By-products Utilization"
            ],
            "E-Commerce & Digital Marketing": [
                "E-Commerce Architecture & Portals", "Search Engine Optimization (SEO & SEM)",
                "Content Marketing & Copywriting", "Web Analytics & Conversion Optimization"
            ],
            "Media Studies": [
                "Digital Media Aesthetics", "Visual Communication & Design",
                "Audio-Visual Storytelling", "Audience Research & Media Analytics"
            ]
        }
    },
    "Professional": {
        depts: {
            "BBA": [
                "Business Organization & Environment", "Financial & Cost Accounting",
                "Marketing & Human Resource Strategy", "Business Law & Taxation"
            ],
            "BBA(CA)": [
                "Modern Programming Concepts", "Relational Database Management",
                "Web Design & E-Commerce Technologies", "Enterprise Resource Planning (ERP)"
            ],
            "BCA": [
                "Structured Programming & C++", "Data Structures & File Organization",
                "Java & Python Enterprise Systems", "Software Engineering & Cloud Services"
            ]
        }
    },
    "Others": {
        depts: {
            "Bachelor of Library & Info Science": [
                "Library Organization, Laws & Management", "Information Sources, Systems & Services",
                "Knowledge Organization (Cataloguing & Classification)", "Library Automation & Digital Libraries"
            ],
            "Master of Library & Info Science": [
                "Advanced Information Systems", "Digital Libraries & Institutional Repositories",
                "Information Retrieval Theory", "Research Methods in Library Science"
            ]
        }
    }
};

// Helper: Generate a reasonable subject/practical code suggestion
function suggestCode(dept, name, isPractical) {
    if (!name) return '';
    var cleanName = name.replace(/\s+Practical$/i, '');
    var words = cleanName.replace(/[^a-zA-Z0-9 ]/g, '').trim().split(/\s+/);
    var acronym = '';
    if (words.length === 1) {
        acronym = words[0].substring(0, 4).toUpperCase();
    } else {
        for (var i = 0; i < words.length && i < 4; i++) {
            acronym += words[i].charAt(0).toUpperCase();
        }
    }
    var deptPrefix = (dept || 'SUB').replace(/[^a-zA-Z0-9]/g, '').substring(0, 3).toUpperCase();
    var code = deptPrefix + '-' + acronym;
    return isPractical ? code + '-P' : code;
}

document.addEventListener('DOMContentLoaded', function () {
    // ═════════════════════════════════════════════════════════════════
    // COMMON ELEMENTS & POPUP
    // ═════════════════════════════════════════════════════════════════
    var successModal    = document.getElementById('successPopupModal');
    var successMessage  = document.getElementById('successPopupMessage');
    var closeSuccessBtn = document.getElementById('closeSuccessPopupBtn');

    function showPopup(msg) {
        if (successMessage) {
            successMessage.innerText = msg;
        }
        if (successModal) successModal.style.display = 'flex';
    }

    if (closeSuccessBtn) {
        closeSuccessBtn.addEventListener('click', function () {
            if (successModal) successModal.style.display = 'none';
        });
    }
    if (successModal) {
        successModal.addEventListener('click', function (e) {
            if (e.target === successModal) successModal.style.display = 'none';
        });
    }

    // ═════════════════════════════════════════════════════════════════
    // 1. ADD SUBJECT MODAL LOGIC
    // ═════════════════════════════════════════════════════════════════
    var openSubjectBtn     = document.getElementById('openAddSubjectBtn');
    var subjectModal       = document.getElementById('subjectModalOverlay');
    var closeSubjectBtn    = document.getElementById('closeSubjectModalBtn');
    var cancelSubjectBtn   = document.getElementById('cancelSubjectBtn');
    var subjectForm        = document.getElementById('addSubjectForm');
    var subjectFormError   = document.getElementById('subjectFormError');
    var saveSubjectBtn     = document.getElementById('saveSubjectBtn');

    var subStreamSelect    = document.getElementById('SubjectStream');
    var subDeptSelect      = document.getElementById('SubjectDepartment');
    var subYearSelect      = document.getElementById('SubjectYear');
    var subNameSelect      = document.getElementById('SubjectName');
    var subCodeInput       = document.getElementById('SubjectCode');

    function closeSubModal() {
        if (subjectModal) subjectModal.style.display = 'none';
        document.body.style.overflow = '';
    }

    if (openSubjectBtn) {
        openSubjectBtn.addEventListener('click', function () {
            if (subjectForm) subjectForm.reset();

            if (subDeptSelect) {
                subDeptSelect.disabled = true;
                subDeptSelect.innerHTML = '<option value="">-- Choose Stream First --</option>';
            }
            if (subYearSelect) {
                subYearSelect.disabled = true;
                subYearSelect.value = '';
            }
            if (subNameSelect) {
                subNameSelect.disabled = true;
                subNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
            }

            var lecTimeInput = document.getElementById('LectureTime');
            if (lecTimeInput) lecTimeInput.value = '10:00 AM - 11:00 AM';

            if (subjectFormError) {
                subjectFormError.style.display = 'none';
                subjectFormError.innerText = '';
            }

            if (subjectModal) subjectModal.style.display = 'block';
            document.body.style.overflow = 'hidden';
        });
    }

    if (closeSubjectBtn)  closeSubjectBtn.addEventListener('click', closeSubModal);
    if (cancelSubjectBtn) cancelSubjectBtn.addEventListener('click', closeSubModal);
    if (subjectModal) {
        subjectModal.addEventListener('click', function (e) {
            if (e.target === subjectModal) closeSubModal();
        });
    }

    // Subject Stream Change
    if (subStreamSelect) {
        subStreamSelect.addEventListener('change', function () {
            var stream = this.value;
            subDeptSelect.innerHTML = '<option value="">-- Choose Department --</option>';
            subDeptSelect.disabled = true;

            subYearSelect.value = '';
            subYearSelect.disabled = true;

            subNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
            subNameSelect.disabled = true;

            if (!stream || !STREAM_DATA[stream]) {
                subDeptSelect.innerHTML = '<option value="">-- Choose Stream First --</option>';
                return;
            }

            var depts = STREAM_DATA[stream].depts;
            Object.keys(depts).forEach(function (dName) {
                var opt = document.createElement('option');
                opt.value = dName;
                opt.textContent = dName;
                subDeptSelect.appendChild(opt);
            });
            subDeptSelect.disabled = false;
        });
    }

    // Subject Dept Change
    if (subDeptSelect) {
        subDeptSelect.addEventListener('change', function () {
            var dept = this.value;
            subYearSelect.value = '';
            subNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
            subNameSelect.disabled = true;

            if (dept) {
                subYearSelect.disabled = false;
            } else {
                subYearSelect.disabled = true;
            }
        });
    }

    // Subject Year Change
    if (subYearSelect) {
        subYearSelect.addEventListener('change', function () {
            var stream = subStreamSelect.value;
            var dept   = subDeptSelect.value;
            var year   = this.value;

            subNameSelect.innerHTML = '<option value="">-- Select Subject --</option>';

            if (!stream || !dept || !year || !STREAM_DATA[stream] || !STREAM_DATA[stream].depts[dept]) {
                subNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
                subNameSelect.disabled = true;
                return;
            }

            var subjectList = STREAM_DATA[stream].depts[dept];
            if (subjectList && subjectList.length > 0) {
                subjectList.forEach(function (sub) {
                    var opt = document.createElement('option');
                    opt.value = sub;
                    opt.textContent = sub;
                    subNameSelect.appendChild(opt);
                });
                subNameSelect.disabled = false;
            } else {
                subNameSelect.innerHTML = '<option value="">-- No Subjects Listed --</option>';
                subNameSelect.disabled = false;
            }
        });
    }

    // Subject Name Change -> auto-suggest code
    if (subNameSelect) {
        subNameSelect.addEventListener('change', function () {
            var selectedSub = this.value;
            var dept = subDeptSelect ? subDeptSelect.value : '';
            if (selectedSub && subCodeInput && !subCodeInput.value) {
                subCodeInput.value = suggestCode(dept, selectedSub, false);
            }
        });
    }

    // Subject Form Submit
    if (subjectForm) {
        subjectForm.addEventListener('submit', function (e) {
            e.preventDefault();
            if (subjectFormError) {
                subjectFormError.style.display = 'none';
                subjectFormError.innerText = '';
            }

            if (!subStreamSelect.value || !subDeptSelect.value || !subYearSelect.value || !subNameSelect.value) {
                showSubError('Please complete all academic details (Stream, Department, Year, and Subject).');
                return;
            }

            var lecTime = (document.getElementById('LectureTime') || {}).value || '';
            if (!lecTime.trim()) {
                showSubError('Lecture time is required.');
                return;
            }

            var formData = new FormData(subjectForm);
            if (saveSubjectBtn) {
                saveSubjectBtn.disabled = true;
                saveSubjectBtn.innerText = 'Saving...';
            }

            fetch('/Home/AddSubject', {
                method: 'POST',
                body: formData
            })
            .then(function (res) { return res.json(); })
            .then(function (data) {
                if (saveSubjectBtn) {
                    saveSubjectBtn.disabled = false;
                    saveSubjectBtn.innerText = '✓ Save Subject';
                }
                if (data && data.success) {
                    closeSubModal();
                    showPopup(data.message || 'Subject added successfully');
                } else {
                    showSubError((data && data.message) ? data.message : 'An error occurred while saving.');
                }
            })
            .catch(function (err) {
                if (saveSubjectBtn) {
                    saveSubjectBtn.disabled = false;
                    saveSubjectBtn.innerText = '✓ Save Subject';
                }
                showSubError('Failed to submit form: ' + err.message);
            });
        });
    }

    function showSubError(msg) {
        if (subjectFormError) {
            subjectFormError.innerText = msg;
            subjectFormError.style.display = 'block';
            subjectFormError.scrollIntoView({ behavior: 'smooth', block: 'center' });
        } else {
            alert(msg);
        }
    }

    // ═════════════════════════════════════════════════════════════════
    // 2. ADD PRACTICAL MODAL LOGIC
    // ═════════════════════════════════════════════════════════════════
    var openPracticalBtn       = document.getElementById('openAddPracticalBtn');
    var practicalModal         = document.getElementById('practicalModalOverlay');
    var closePracticalBtn      = document.getElementById('closePracticalModalBtn');
    var cancelPracticalBtn     = document.getElementById('cancelPracticalBtn');
    var practicalForm          = document.getElementById('addPracticalForm');
    var practicalFormError     = document.getElementById('practicalFormError');
    var savePracticalBtn       = document.getElementById('savePracticalBtn');

    var pracStreamSelect       = document.getElementById('PracticalStream');
    var pracDeptSelect         = document.getElementById('PracticalDepartment');
    var pracYearSelect         = document.getElementById('PracticalYear');
    var pracNameSelect         = document.getElementById('PracticalSubjectNameSelect');
    var pracCodeInput          = document.getElementById('StandalonePracticalCode');

    function closePracModal() {
        if (practicalModal) practicalModal.style.display = 'none';
        document.body.style.overflow = '';
    }

    if (openPracticalBtn) {
        openPracticalBtn.addEventListener('click', function () {
            if (practicalForm) practicalForm.reset();

            if (pracDeptSelect) {
                pracDeptSelect.disabled = true;
                pracDeptSelect.innerHTML = '<option value="">-- Choose Stream First --</option>';
            }
            if (pracYearSelect) {
                pracYearSelect.disabled = true;
                pracYearSelect.value = '';
            }
            if (pracNameSelect) {
                pracNameSelect.disabled = true;
                pracNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
            }

            var pracTimeInput = document.getElementById('StandalonePracticalTiming');
            if (pracTimeInput) pracTimeInput.value = '02:00 PM - 04:00 PM';

            if (practicalFormError) {
                practicalFormError.style.display = 'none';
                practicalFormError.innerText = '';
            }

            if (practicalModal) practicalModal.style.display = 'block';
            document.body.style.overflow = 'hidden';
        });
    }

    if (closePracticalBtn)  closePracticalBtn.addEventListener('click', closePracModal);
    if (cancelPracticalBtn) cancelPracticalBtn.addEventListener('click', closePracModal);
    if (practicalModal) {
        practicalModal.addEventListener('click', function (e) {
            if (e.target === practicalModal) closePracModal();
        });
    }

    // Practical Stream Change
    if (pracStreamSelect) {
        pracStreamSelect.addEventListener('change', function () {
            var stream = this.value;
            pracDeptSelect.innerHTML = '<option value="">-- Choose Department --</option>';
            pracDeptSelect.disabled = true;

            pracYearSelect.value = '';
            pracYearSelect.disabled = true;

            pracNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
            pracNameSelect.disabled = true;

            if (!stream || !STREAM_DATA[stream]) {
                pracDeptSelect.innerHTML = '<option value="">-- Choose Stream First --</option>';
                return;
            }

            var depts = STREAM_DATA[stream].depts;
            Object.keys(depts).forEach(function (dName) {
                var opt = document.createElement('option');
                opt.value = dName;
                opt.textContent = dName;
                pracDeptSelect.appendChild(opt);
            });
            pracDeptSelect.disabled = false;
        });
    }

    // Practical Dept Change
    if (pracDeptSelect) {
        pracDeptSelect.addEventListener('change', function () {
            var dept = this.value;
            pracYearSelect.value = '';
            pracNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
            pracNameSelect.disabled = true;

            if (dept) {
                pracYearSelect.disabled = false;
            } else {
                pracYearSelect.disabled = true;
            }
        });
    }

    // Practical Year Change -> populate Practical Subjects
    if (pracYearSelect) {
        pracYearSelect.addEventListener('change', function () {
            var stream = pracStreamSelect.value;
            var dept   = pracDeptSelect.value;
            var year   = this.value;

            pracNameSelect.innerHTML = '<option value="">-- Select Practical Subject --</option>';

            if (!stream || !dept || !year || !STREAM_DATA[stream] || !STREAM_DATA[stream].depts[dept]) {
                pracNameSelect.innerHTML = '<option value="">-- Choose Year First --</option>';
                pracNameSelect.disabled = true;
                return;
            }

            var subjectList = STREAM_DATA[stream].depts[dept];
            if (subjectList && subjectList.length > 0) {
                subjectList.forEach(function (sub) {
                    var opt = document.createElement('option');
                    var pName = sub + ' Practical';
                    opt.value = pName;
                    opt.textContent = pName;
                    pracNameSelect.appendChild(opt);
                });
                pracNameSelect.disabled = false;
            } else {
                pracNameSelect.innerHTML = '<option value="">-- No Subjects Listed --</option>';
                pracNameSelect.disabled = false;
            }
        });
    }

    // Practical Subject Change -> auto-suggest practical code
    if (pracNameSelect) {
        pracNameSelect.addEventListener('change', function () {
            var selectedPrac = this.value;
            var dept = pracDeptSelect ? pracDeptSelect.value : '';
            if (selectedPrac && pracCodeInput) {
                pracCodeInput.value = suggestCode(dept, selectedPrac, true);
            }
        });
    }

    // Practical Form Submit
    if (practicalForm) {
        practicalForm.addEventListener('submit', function (e) {
            e.preventDefault();
            if (practicalFormError) {
                practicalFormError.style.display = 'none';
                practicalFormError.innerText = '';
            }

            if (!pracStreamSelect.value || !pracDeptSelect.value || !pracYearSelect.value || !pracNameSelect.value) {
                showPracError('Please complete all academic details (Stream, Department, Year, and Practical Subject).');
                return;
            }

            var pracTime = (document.getElementById('StandalonePracticalTiming') || {}).value || '';
            if (!pracTime.trim()) {
                showPracError('Time of Practical is required.');
                return;
            }

            var formData = new FormData(practicalForm);
            if (savePracticalBtn) {
                savePracticalBtn.disabled = true;
                savePracticalBtn.innerText = 'Saving...';
            }

            fetch('/Home/AddPractical', {
                method: 'POST',
                body: formData
            })
            .then(function (res) { return res.json(); })
            .then(function (data) {
                if (savePracticalBtn) {
                    savePracticalBtn.disabled = false;
                    savePracticalBtn.innerText = '✓ Save Practical';
                }
                if (data && data.success) {
                    closePracModal();
                    showPopup(data.message || 'Practical subject added successfully');
                } else {
                    showPracError((data && data.message) ? data.message : 'An error occurred while saving.');
                }
            })
            .catch(function (err) {
                if (savePracticalBtn) {
                    savePracticalBtn.disabled = false;
                    savePracticalBtn.innerText = '✓ Save Practical';
                }
                showPracError('Failed to submit form: ' + err.message);
            });
        });
    }

    function showPracError(msg) {
        if (practicalFormError) {
            practicalFormError.innerText = msg;
            practicalFormError.style.display = 'block';
            practicalFormError.scrollIntoView({ behavior: 'smooth', block: 'center' });
        } else {
            alert(msg);
        }
    }
});
