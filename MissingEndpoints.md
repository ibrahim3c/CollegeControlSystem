# Missing Endpoints - CollegeControlSystem

Generated on: 2026-05-01

## 9. Grades

| #   | HTTP Method | Route                                 | Action        | Description                           |
| --- | ----------- | ------------------------------------- | ------------- | ------------------------------------- |
| 1   | GET         | `api/registrations/{id}/grade`        | `GetGrade`    | Get grade for a specific registration |
| 2   | POST        | `api/registrations/{id}/grade/appeal` | `AppealGrade` | Submit a grade appeal                 |

---

## 10. Academic Reports

| #   | HTTP Method | Route                                 | Action                 | Description                           |
| --- | ----------- | ------------------------------------- | ---------------------- | ------------------------------------- |
| 1   | GET         | `api/control/statistics`              | `GetStatistics`        | General academic statistics dashboard |
| 2   | GET         | `api/students/{id}/academic-history`  | `GetAcademicHistory`   | Full semester-by-semester history     |
| 3   | GET         | `api/course-offerings/{id}/analytics` | `GetOfferingAnalytics` | Enrollment analytics per offering     |

---
