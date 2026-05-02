# Missing Endpoints - CollegeControlSystem

Generated on: 2026-05-01



## 5. Courses

| # | HTTP Method | Route | Action | Description |
|---|-------------|-------|--------|-------------|
| 1 | PUT | `api/courses/{id}` | `UpdateCourse` | Update course details |
| 2 | DELETE | `api/courses/{id}` | `DeleteCourse` | Delete course |
| 3 | GET | `api/courses/{id}/prerequisites` | `GetPrerequisites` | Get all prerequisites for a course |

---

## 6. CourseOfferings

| # | HTTP Method | Route | Action | Description |
|---|-------------|-------|--------|-------------|
| 1 | GET | `api/course-offerings/{id}` | `GetById` | Get offering by ID |
| 2 | DELETE | `api/course-offerings/{id}` | `DeleteOffering` | Delete offering |
| 3 | PUT | `api/course-offerings/{id}/cancel` | `CancelOffering` | Cancel an offering |

---

## 7. Faculties

| # | HTTP Method | Route | Action | Description |
|---|-------------|-------|--------|-------------|
| 1 | DELETE | `api/faculty/{id}` | `DeleteFaculty` | Delete faculty member |
| 2 | GET | `api/faculty/{id}/schedule` | `GetFacultySchedule` | Get faculty teaching schedule |

---

## 8. Identity / Accounts

| # | HTTP Method | Route | Action | Description |
|---|-------------|-------|--------|-------------|
| 1 | POST | `api/accounts/register` | `Register` | User registration (command exists, endpoint commented out) |
| 2 | PUT | `api/accounts/change-password` | `ChangePassword` | Change password for authenticated user |
| 3 | POST | `api/accounts/logout` | `Logout` | Invalidate all tokens |

---

## 9. Grades

| # | HTTP Method | Route | Action | Description |
|---|-------------|-------|--------|-------------|
| 1 | GET | `api/registrations/{id}/grade` | `GetGrade` | Get grade for a specific registration |
| 2 | POST | `api/registrations/{id}/grade/appeal` | `AppealGrade` | Submit a grade appeal |

---

## 10. Academic Reports

| # | HTTP Method | Route | Action | Description |
|---|-------------|-------|--------|-------------|
| 1 | GET | `api/control/statistics` | `GetStatistics` | General academic statistics dashboard |
| 2 | GET | `api/students/{id}/academic-history` | `GetAcademicHistory` | Full semester-by-semester history |
| 3 | GET | `api/course-offerings/{id}/analytics` | `GetOfferingAnalytics` | Enrollment analytics per offering |

---

## Summary

| Category | Missing Endpoints |
|----------|------------------|
| Students | 2 |
| Departments | 2 |
| Programs | 3 |
| Registrations | 3 |
| Courses | 3 |
| CourseOfferings | 3 |
| Faculties | 2 |
| Identity/Accounts | 3 |
| Grades | 2 |
| Academic Reports | 3 |
| **Total** | **26** |
