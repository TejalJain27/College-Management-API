// College Management System - Frontend App Logic
const API_BASE = '/api';

// Cache for dropdowns
let cachedStudents = [];
let cachedCourses = [];
let cachedFaculty = [];

// Initialize Dashboard on DOM Load
document.addEventListener('DOMContentLoaded', () => {
    setupNavigation();
    loadDashboard();
    preloadDropdownData();
});

// Utility: Debounce for search
function debounce(func, wait) {
    let timeout;
    return function (...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}

// Navigation Tab Handler
function setupNavigation() {
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(item => {
        item.addEventListener('click', () => {
            const tabName = item.getAttribute('data-tab');
            switchTab(tabName);
        });
    });
}

function switchTab(tabName) {
    document.querySelectorAll('.nav-item').forEach(el => el.classList.remove('active'));
    document.querySelectorAll('.tab-content').forEach(el => el.classList.remove('active'));

    const selectedBtn = document.querySelector(`.nav-item[data-tab="${tabName}"]`);
    const selectedTab = document.getElementById(`tab-${tabName}`);

    if (selectedBtn && selectedTab) {
        selectedBtn.classList.add('active');
        selectedTab.classList.add('active');
    }

    // Update Header
    const titles = {
        'dashboard': ['Dashboard Overview', 'Real-time academic evaluation & administrative control'],
        'students': ['Student Directory', 'Manage student records, search, and enrollments'],
        'courses': ['Course Management', 'View academic catalog and faculty course assignments'],
        'faculty': ['Faculty Directory', 'Manage professors, associate professors, and department assignments'],
        'enrollments': ['Course Enrollments', 'Register students into active courses'],
        'attendance': ['Attendance Records', 'Track and evaluate daily student attendance percentages'],
        'marks': ['Marks & Grade Book', 'Record marks and automatically calculate academic grades']
    };

    if (titles[tabName]) {
        document.getElementById('page-title').textContent = titles[tabName][0];
        document.getElementById('page-subtitle').textContent = titles[tabName][1];
    }

    // Load Tab Data
    switch (tabName) {
        case 'dashboard': loadDashboard(); break;
        case 'students': loadStudents(); break;
        case 'courses': loadCourses(); break;
        case 'faculty': loadFaculty(); break;
        case 'enrollments': loadEnrollments(); break;
        case 'attendance': loadAttendance(); break;
        case 'marks': loadMarks(); break;
    }
}

// Toast Notifications
function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-triangle';
    toast.innerHTML = `<i class="fa-solid ${icon}"></i> <span>${message}</span>`;

    container.appendChild(toast);
    setTimeout(() => {
        toast.remove();
    }, 4000);
}

// Preload Select Dropdown Options
async function preloadDropdownData() {
    try {
        const [sRes, cRes, fRes] = await Promise.all([
            fetch(`${API_BASE}/students`),
            fetch(`${API_BASE}/courses`),
            fetch(`${API_BASE}/faculty`)
        ]);

        if (sRes.ok) cachedStudents = await sRes.json();
        if (cRes.ok) cachedCourses = await cRes.json();
        if (fRes.ok) cachedFaculty = await fRes.json();

        populateSelectOptions();
    } catch (err) {
        console.error('Failed to preload dropdown data:', err);
    }
}

function populateSelectOptions() {
    // Populate Student dropdowns
    const studentSelects = ['enrollment-student', 'attendance-student', 'attendance-filter-student', 'mark-student'];
    studentSelects.forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;
        const currentVal = el.value;
        el.innerHTML = id.includes('filter') ? '<option value="">All Students</option>' : '<option value="">-- Select Student --</option>';
        cachedStudents.forEach(s => {
            el.innerHTML += `<option value="${s.id}">${s.studentNumber} - ${s.firstName} ${s.lastName} (${s.department})</option>`;
        });
        if (currentVal) el.value = currentVal;
    });

    // Populate Course dropdowns
    const courseSelects = ['enrollment-course', 'attendance-course', 'attendance-filter-course', 'mark-course'];
    courseSelects.forEach(id => {
        const el = document.getElementById(id);
        if (!el) return;
        const currentVal = el.value;
        el.innerHTML = id.includes('filter') ? '<option value="">All Courses</option>' : '<option value="">-- Select Course --</option>';
        cachedCourses.forEach(c => {
            el.innerHTML += `<option value="${c.id}">${c.courseCode} - ${c.courseName}</option>`;
        });
        if (currentVal) el.value = currentVal;
    });

    // Populate Faculty dropdown in Course Modal
    const facultyEl = document.getElementById('course-faculty');
    if (facultyEl) {
        facultyEl.innerHTML = '<option value="">-- Unassigned --</option>';
        cachedFaculty.forEach(f => {
            facultyEl.innerHTML += `<option value="${f.id}">${f.firstName} ${f.lastName} (${f.department})</option>`;
        });
    }
}

// ---------------- DASHBOARD ----------------
async function loadDashboard() {
    try {
        const res = await fetch(`${API_BASE}/dashboard`);
        if (!res.ok) throw new Error('Failed to load dashboard metrics');
        const data = await res.json();

        document.getElementById('stat-students').textContent = data.totalStudents;
        document.getElementById('stat-courses').textContent = data.totalCourses;
        document.getElementById('stat-faculty').textContent = data.totalFaculty;
        document.getElementById('stat-enrollments').textContent = data.totalEnrollments;

        // Render Recent Students
        const tbody = document.getElementById('recent-students-tbody');
        if (data.recentStudents && data.recentStudents.length > 0) {
            tbody.innerHTML = data.recentStudents.map(s => `
                <tr>
                    <td><span class="badge badge-blue">${s.studentNumber}</span></td>
                    <td><strong>${s.firstName} ${s.lastName}</strong></td>
                    <td>${s.department}</td>
                    <td>Semester ${s.semester}</td>
                    <td>${s.email}</td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center">No recent students found</td></tr>`;
        }

        // Render Department Stats
        const deptList = document.getElementById('dept-stats-list');
        if (data.departmentStats && data.departmentStats.length > 0) {
            deptList.innerHTML = data.departmentStats.map(d => `
                <div class="dept-item">
                    <span class="dept-name">${d.department}</span>
                    <div class="dept-badges">
                        <span class="badge badge-blue" title="Students">${d.studentCount} Students</span>
                        <span class="badge badge-purple" title="Courses">${d.courseCount} Courses</span>
                        <span class="badge badge-emerald" title="Faculty">${d.facultyCount} Faculty</span>
                    </div>
                </div>
            `).join('');
        } else {
            deptList.innerHTML = `<div class="text-center py-3">No department statistics available</div>`;
        }
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- STUDENTS ----------------
async function loadStudents() {
    const search = document.getElementById('student-search').value;
    const department = document.getElementById('student-filter-dept').value;

    let url = `${API_BASE}/students?`;
    if (search) url += `search=${encodeURIComponent(search)}&`;
    if (department) url += `department=${encodeURIComponent(department)}&`;

    try {
        const res = await fetch(url);
        if (!res.ok) throw new Error('Failed to load students');
        const students = await res.json();
        cachedStudents = students;
        populateSelectOptions();

        const tbody = document.getElementById('students-tbody');
        if (students.length === 0) {
            tbody.innerHTML = `<tr><td colspan="7" class="text-center">No students found matching your criteria.</td></tr>`;
            return;
        }

        tbody.innerHTML = students.map(s => `
            <tr>
                <td><span class="badge badge-blue">${s.studentNumber}</span></td>
                <td><strong>${s.firstName} ${s.lastName}</strong></td>
                <td>${s.email}</td>
                <td>${s.phone || '-'}</td>
                <td>${s.department}</td>
                <td>Semester ${s.semester}</td>
                <td>
                    <button class="btn btn-sm btn-icon btn-edit" onclick="editStudent(${s.id})" title="Edit Student"><i class="fa-solid fa-pen"></i></button>
                    <button class="btn btn-sm btn-icon btn-delete" onclick="deleteStudent(${s.id})" title="Delete Student"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function openStudentModal(student = null) {
    document.getElementById('student-form').reset();
    document.getElementById('student-id').value = '';
    
    if (student) {
        document.getElementById('student-modal-title').textContent = 'Edit Student';
        document.getElementById('student-id').value = student.id;
        document.getElementById('student-number').value = student.studentNumber;
        document.getElementById('student-number').disabled = true;
        document.getElementById('student-email').value = student.email;
        document.getElementById('student-firstname').value = student.firstName;
        document.getElementById('student-lastname').value = student.lastName;
        document.getElementById('student-phone').value = student.phone || '';
        document.getElementById('student-dob').value = student.dateOfBirth ? student.dateOfBirth.split('T')[0] : '';
        document.getElementById('student-department').value = student.department;
        document.getElementById('student-semester').value = student.semester;
    } else {
        document.getElementById('student-modal-title').textContent = 'Add New Student';
        document.getElementById('student-number').disabled = false;
        document.getElementById('student-dob').value = '2004-01-01';
    }

    openModal('student-modal');
}

async function editStudent(id) {
    try {
        const res = await fetch(`${API_BASE}/students/${id}`);
        if (!res.ok) throw new Error('Student not found');
        const student = await res.json();
        openStudentModal(student);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function saveStudent(event) {
    event.preventDefault();
    const id = document.getElementById('student-id').value;

    const payload = {
        studentNumber: document.getElementById('student-number').value.trim(),
        firstName: document.getElementById('student-firstname').value.trim(),
        lastName: document.getElementById('student-lastname').value.trim(),
        email: document.getElementById('student-email').value.trim(),
        phone: document.getElementById('student-phone').value.trim(),
        dateOfBirth: document.getElementById('student-dob').value,
        department: document.getElementById('student-department').value,
        semester: parseInt(document.getElementById('student-semester').value)
    };

    const isEdit = !!id;
    const url = isEdit ? `${API_BASE}/students/${id}` : `${API_BASE}/students`;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();

        if (!res.ok) {
            throw new Error(data.message || 'Error saving student');
        }

        showToast(isEdit ? 'Student updated successfully!' : 'Student created successfully!');
        closeModal('student-modal');
        loadStudents();
        preloadDropdownData();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteStudent(id) {
    if (!confirm('Are you sure you want to delete this student? Associated records will be cleaned up.')) return;
    try {
        const res = await fetch(`${API_BASE}/students/${id}`, { method: 'DELETE' });
        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || 'Failed to delete student');
        }
        showToast('Student deleted successfully!');
        loadStudents();
        preloadDropdownData();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- COURSES ----------------
async function loadCourses() {
    try {
        const res = await fetch(`${API_BASE}/courses`);
        if (!res.ok) throw new Error('Failed to load courses');
        const courses = await res.json();
        cachedCourses = courses;
        populateSelectOptions();

        const tbody = document.getElementById('courses-tbody');
        if (courses.length === 0) {
            tbody.innerHTML = `<tr><td colspan="8" class="text-center">No courses found.</td></tr>`;
            return;
        }

        tbody.innerHTML = courses.map(c => `
            <tr>
                <td><span class="badge badge-purple">${c.courseCode}</span></td>
                <td><strong>${c.courseName}</strong></td>
                <td>${c.department}</td>
                <td>${c.credits} Credits</td>
                <td>Semester ${c.semester}</td>
                <td>${c.facultyName ? `<span class="badge badge-emerald"><i class="fa-solid fa-user-tie"></i> ${c.facultyName}</span>` : '<span class="text-muted">Unassigned</span>'}</td>
                <td><span class="badge badge-blue">${c.enrolledStudentsCount} Enrolled</span></td>
                <td>
                    <button class="btn btn-sm btn-icon btn-edit" onclick="editCourse(${c.id})" title="Edit Course"><i class="fa-solid fa-pen"></i></button>
                    <button class="btn btn-sm btn-icon btn-delete" onclick="deleteCourse(${c.id})" title="Delete Course"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function openCourseModal(course = null) {
    document.getElementById('course-form').reset();
    document.getElementById('course-id').value = '';

    if (course) {
        document.getElementById('course-modal-title').textContent = 'Edit Course';
        document.getElementById('course-id').value = course.id;
        document.getElementById('course-code').value = course.courseCode;
        document.getElementById('course-code').disabled = true;
        document.getElementById('course-name').value = course.courseName;
        document.getElementById('course-credits').value = course.credits;
        document.getElementById('course-department').value = course.department;
        document.getElementById('course-semester').value = course.semester;
        document.getElementById('course-faculty').value = course.facultyId || '';
    } else {
        document.getElementById('course-modal-title').textContent = 'Add New Course';
        document.getElementById('course-code').disabled = false;
    }

    openModal('course-modal');
}

async function editCourse(id) {
    try {
        const res = await fetch(`${API_BASE}/courses/${id}`);
        if (!res.ok) throw new Error('Course not found');
        const course = await res.json();
        openCourseModal(course);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function saveCourse(event) {
    event.preventDefault();
    const id = document.getElementById('course-id').value;

    const facultyIdVal = document.getElementById('course-faculty').value;

    const payload = {
        courseCode: document.getElementById('course-code').value.trim(),
        courseName: document.getElementById('course-name').value.trim(),
        credits: parseInt(document.getElementById('course-credits').value),
        department: document.getElementById('course-department').value,
        semester: parseInt(document.getElementById('course-semester').value),
        facultyId: facultyIdVal ? parseInt(facultyIdVal) : null
    };

    const isEdit = !!id;
    const url = isEdit ? `${API_BASE}/courses/${id}` : `${API_BASE}/courses`;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Error saving course');

        showToast(isEdit ? 'Course updated successfully!' : 'Course created successfully!');
        closeModal('course-modal');
        loadCourses();
        preloadDropdownData();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteCourse(id) {
    if (!confirm('Are you sure you want to delete this course?')) return;
    try {
        const res = await fetch(`${API_BASE}/courses/${id}`, { method: 'DELETE' });
        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || 'Failed to delete course');
        }
        showToast('Course deleted successfully!');
        loadCourses();
        preloadDropdownData();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- FACULTY ----------------
async function loadFaculty() {
    try {
        const res = await fetch(`${API_BASE}/faculty`);
        if (!res.ok) throw new Error('Failed to load faculty list');
        const faculty = await res.json();
        cachedFaculty = faculty;
        populateSelectOptions();

        const tbody = document.getElementById('faculty-tbody');
        if (faculty.length === 0) {
            tbody.innerHTML = `<tr><td colspan="7" class="text-center">No faculty members found.</td></tr>`;
            return;
        }

        tbody.innerHTML = faculty.map(f => `
            <tr>
                <td><span class="badge badge-emerald">${f.employeeNumber}</span></td>
                <td><strong>${f.firstName} ${f.lastName}</strong></td>
                <td>${f.email}</td>
                <td>${f.department}</td>
                <td><span class="badge badge-purple">${f.designation}</span></td>
                <td><span class="badge badge-blue">${f.courseCount} Courses</span></td>
                <td>
                    <button class="btn btn-sm btn-icon btn-edit" onclick="editFaculty(${f.id})" title="Edit Faculty"><i class="fa-solid fa-pen"></i></button>
                    <button class="btn btn-sm btn-icon btn-delete" onclick="deleteFaculty(${f.id})" title="Delete Faculty"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function openFacultyModal(faculty = null) {
    document.getElementById('faculty-form').reset();
    document.getElementById('faculty-id').value = '';

    if (faculty) {
        document.getElementById('faculty-modal-title').textContent = 'Edit Faculty Member';
        document.getElementById('faculty-id').value = faculty.id;
        document.getElementById('faculty-empnumber').value = faculty.employeeNumber;
        document.getElementById('faculty-empnumber').disabled = true;
        document.getElementById('faculty-email').value = faculty.email;
        document.getElementById('faculty-firstname').value = faculty.firstName;
        document.getElementById('faculty-lastname').value = faculty.lastName;
        document.getElementById('faculty-department').value = faculty.department;
        document.getElementById('faculty-designation').value = faculty.designation;
    } else {
        document.getElementById('faculty-modal-title').textContent = 'Add Faculty Member';
        document.getElementById('faculty-empnumber').disabled = false;
    }

    openModal('faculty-modal');
}

async function editFaculty(id) {
    try {
        const res = await fetch(`${API_BASE}/faculty/${id}`);
        if (!res.ok) throw new Error('Faculty member not found');
        const faculty = await res.json();
        openFacultyModal(faculty);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function saveFaculty(event) {
    event.preventDefault();
    const id = document.getElementById('faculty-id').value;

    const payload = {
        employeeNumber: document.getElementById('faculty-empnumber').value.trim(),
        firstName: document.getElementById('faculty-firstname').value.trim(),
        lastName: document.getElementById('faculty-lastname').value.trim(),
        email: document.getElementById('faculty-email').value.trim(),
        department: document.getElementById('faculty-department').value,
        designation: document.getElementById('faculty-designation').value.trim()
    };

    const isEdit = !!id;
    const url = isEdit ? `${API_BASE}/faculty/${id}` : `${API_BASE}/faculty`;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Error saving faculty');

        showToast(isEdit ? 'Faculty member updated!' : 'Faculty member created!');
        closeModal('faculty-modal');
        loadFaculty();
        preloadDropdownData();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteFaculty(id) {
    if (!confirm('Are you sure you want to delete this faculty member?')) return;
    try {
        const res = await fetch(`${API_BASE}/faculty/${id}`, { method: 'DELETE' });
        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || 'Failed to delete faculty');
        }
        showToast('Faculty deleted successfully!');
        loadFaculty();
        preloadDropdownData();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- ENROLLMENTS ----------------
async function loadEnrollments() {
    try {
        const res = await fetch(`${API_BASE}/enrollments`);
        if (!res.ok) throw new Error('Failed to load enrollments');
        const enrollments = await res.json();

        const tbody = document.getElementById('enrollments-tbody');
        if (enrollments.length === 0) {
            tbody.innerHTML = `<tr><td colspan="7" class="text-center">No enrollments recorded.</td></tr>`;
            return;
        }

        tbody.innerHTML = enrollments.map(e => `
            <tr>
                <td><span class="badge badge-blue">${e.studentNumber}</span></td>
                <td><strong>${e.studentName}</strong></td>
                <td><span class="badge badge-purple">${e.courseCode}</span></td>
                <td>${e.courseName}</td>
                <td>${new Date(e.enrollmentDate).toLocaleDateString()}</td>
                <td><span class="badge ${e.status === 'Active' ? 'badge-emerald' : 'badge-amber'}">${e.status}</span></td>
                <td>
                    <button class="btn btn-sm btn-icon btn-delete" onclick="deleteEnrollment(${e.id})" title="Delete Enrollment"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function openEnrollmentModal() {
    document.getElementById('enrollment-form').reset();
    openModal('enrollment-modal');
}

async function saveEnrollment(event) {
    event.preventDefault();

    const payload = {
        studentId: parseInt(document.getElementById('enrollment-student').value),
        courseId: parseInt(document.getElementById('enrollment-course').value),
        status: document.getElementById('enrollment-status').value
    };

    try {
        const res = await fetch(`${API_BASE}/enrollments`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Error creating enrollment');

        showToast('Student enrolled successfully!');
        closeModal('enrollment-modal');
        loadEnrollments();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteEnrollment(id) {
    if (!confirm('Are you sure you want to remove this enrollment?')) return;
    try {
        const res = await fetch(`${API_BASE}/enrollments/${id}`, { method: 'DELETE' });
        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || 'Failed to delete enrollment');
        }
        showToast('Enrollment removed!');
        loadEnrollments();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- ATTENDANCE ----------------
async function loadAttendance() {
    const studentId = document.getElementById('attendance-filter-student').value;
    const courseId = document.getElementById('attendance-filter-course').value;

    let url = `${API_BASE}/attendance?`;
    if (studentId) url += `studentId=${studentId}&`;
    if (courseId) url += `courseId=${courseId}&`;

    try {
        const res = await fetch(url);
        if (!res.ok) throw new Error('Failed to load attendance');
        const records = await res.json();

        const tbody = document.getElementById('attendance-tbody');
        if (records.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center">No attendance records found.</td></tr>`;
            return;
        }

        tbody.innerHTML = records.map(a => `
            <tr>
                <td>${new Date(a.date).toLocaleDateString()}</td>
                <td><strong>${a.studentName}</strong> (${a.studentNumber})</td>
                <td><span class="badge badge-purple">${a.courseCode}</span> ${a.courseName}</td>
                <td>
                    ${a.isPresent 
                        ? '<span class="badge badge-emerald"><i class="fa-solid fa-check"></i> Present</span>' 
                        : '<span class="badge badge-rose"><i class="fa-solid fa-xmark"></i> Absent</span>'}
                </td>
                <td>
                    <button class="btn btn-sm btn-icon btn-delete" onclick="deleteAttendance(${a.id})" title="Delete Record"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function openAttendanceModal() {
    document.getElementById('attendance-form').reset();
    document.getElementById('attendance-date').value = new Date().toISOString().split('T')[0];
    openModal('attendance-modal');
}

async function saveAttendance(event) {
    event.preventDefault();

    const payload = {
        studentId: parseInt(document.getElementById('attendance-student').value),
        courseId: parseInt(document.getElementById('attendance-course').value),
        date: document.getElementById('attendance-date').value,
        isPresent: document.getElementById('attendance-status').value === 'true'
    };

    try {
        const res = await fetch(`${API_BASE}/attendance`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Error saving attendance');

        showToast('Attendance logged successfully!');
        closeModal('attendance-modal');
        loadAttendance();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteAttendance(id) {
    if (!confirm('Are you sure you want to delete this attendance record?')) return;
    try {
        const res = await fetch(`${API_BASE}/attendance/${id}`, { method: 'DELETE' });
        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || 'Failed to delete attendance');
        }
        showToast('Attendance record deleted!');
        loadAttendance();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- MARKS & GRADES ----------------
async function loadMarks() {
    try {
        const res = await fetch(`${API_BASE}/marks`);
        if (!res.ok) throw new Error('Failed to load marks');
        const marks = await res.json();

        const tbody = document.getElementById('marks-tbody');
        if (marks.length === 0) {
            tbody.innerHTML = `<tr><td colspan="9" class="text-center">No mark sheets found.</td></tr>`;
            return;
        }

        tbody.innerHTML = marks.map(m => `
            <tr>
                <td><span class="badge badge-blue">${m.studentNumber}</span></td>
                <td><strong>${m.studentName}</strong></td>
                <td><span class="badge badge-purple">${m.courseCode}</span></td>
                <td>${m.courseName}</td>
                <td><strong>${m.marksObtained}</strong></td>
                <td>${m.maximumMarks}</td>
                <td>${m.percentage}%</td>
                <td><span class="badge ${getGradeBadgeClass(m.grade)} badge-grade">${m.grade}</span></td>
                <td>
                    <button class="btn btn-sm btn-icon btn-edit" onclick="editMark(${m.id})" title="Edit Marks"><i class="fa-solid fa-pen"></i></button>
                    <button class="btn btn-sm btn-icon btn-delete" onclick="deleteMark(${m.id})" title="Delete Marks"><i class="fa-solid fa-trash"></i></button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        showToast(err.message, 'error');
    }
}

function getGradeBadgeClass(grade) {
    switch (grade) {
        case 'A+': case 'A': return 'badge-emerald';
        case 'B': return 'badge-blue';
        case 'C': return 'badge-purple';
        case 'D': return 'badge-amber';
        default: return 'badge-rose';
    }
}

function previewGrade() {
    const obtained = parseFloat(document.getElementById('mark-obtained').value) || 0;
    const max = parseFloat(document.getElementById('mark-max').value) || 100;
    const badge = document.getElementById('grade-preview-badge');

    if (max <= 0 || obtained < 0 || obtained > max) {
        badge.textContent = 'Invalid Marks';
        badge.className = 'badge badge-rose badge-grade';
        return;
    }

    const pct = (obtained / max) * 100;
    let grade = 'F';
    if (pct >= 90) grade = 'A+';
    else if (pct >= 80) grade = 'A';
    else if (pct >= 70) grade = 'B';
    else if (pct >= 60) grade = 'C';
    else if (pct >= 50) grade = 'D';

    badge.textContent = `${grade} (${pct.toFixed(1)}%)`;
    badge.className = `badge ${getGradeBadgeClass(grade)} badge-grade`;
}

function openMarkModal(mark = null) {
    document.getElementById('mark-form').reset();
    document.getElementById('mark-id').value = '';

    if (mark) {
        document.getElementById('mark-modal-title').textContent = 'Edit Student Marks';
        document.getElementById('mark-id').value = mark.id;
        document.getElementById('mark-student').value = mark.studentId;
        document.getElementById('mark-course').value = mark.courseId;
        document.getElementById('mark-obtained').value = mark.marksObtained;
        document.getElementById('mark-max').value = mark.maximumMarks;
    } else {
        document.getElementById('mark-modal-title').textContent = 'Enter Student Marks';
        document.getElementById('mark-max').value = 100;
    }

    previewGrade();
    openModal('mark-modal');
}

async function editMark(id) {
    try {
        const res = await fetch(`${API_BASE}/marks/${id}`);
        if (!res.ok) throw new Error('Mark record not found');
        const mark = await res.json();
        openMarkModal(mark);
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function saveMark(event) {
    event.preventDefault();
    const id = document.getElementById('mark-id').value;

    const payload = {
        studentId: parseInt(document.getElementById('mark-student').value),
        courseId: parseInt(document.getElementById('mark-course').value),
        marksObtained: parseFloat(document.getElementById('mark-obtained').value),
        maximumMarks: parseFloat(document.getElementById('mark-max').value)
    };

    const isEdit = !!id;
    const url = isEdit ? `${API_BASE}/marks/${id}` : `${API_BASE}/marks`;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Error saving marks');

        showToast(isEdit ? 'Marks updated successfully!' : 'Marks entered successfully!');
        closeModal('mark-modal');
        loadMarks();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

async function deleteMark(id) {
    if (!confirm('Are you sure you want to delete this mark record?')) return;
    try {
        const res = await fetch(`${API_BASE}/marks/${id}`, { method: 'DELETE' });
        if (!res.ok) {
            const data = await res.json();
            throw new Error(data.message || 'Failed to delete mark record');
        }
        showToast('Mark record deleted!');
        loadMarks();
    } catch (err) {
        showToast(err.message, 'error');
    }
}

// ---------------- MODAL UTILS ----------------
function openModal(id) {
    document.getElementById(id).classList.add('active');
}

function closeModal(id) {
    document.getElementById(id).classList.remove('active');
}
