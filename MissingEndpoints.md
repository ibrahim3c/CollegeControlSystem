# Missing Endpoints - CollegeControlSystem

Generated on: 2026-05-01

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
