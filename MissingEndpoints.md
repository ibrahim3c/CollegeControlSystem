# Missing Endpoints

Endpoints needed but not yet implemented, mapped to PRD requirements and priority.

---

## 1. Registration Period Management (FR-REG)

The PRD (REG-1, REG-5) describes an **open registration period** concept, but there is no way to manage it.

| #   | Method | Route                               | Action                     | PRD Ref | Priority |
| --- | ------ | ----------------------------------- | -------------------------- | ------- | -------- |
| 1   | `POST` | `/api/registration-periods`         | Create registration period | REG-1   | **High** |
| 2   | `PUT`  | `/api/registration-periods/{id}`    | Update period dates/status | REG-1   | **High** |
| 3   | `GET`  | `/api/registration-periods/current` | Get current active period  | REG-1   | **High** |
| 4   | `GET`  | `/api/registration-periods`         | List all periods           | REG-1   | Medium   |

> **Why:** Without this, registration is always "open" and the `RegisterCourse` handler cannot validate whether registration is currently allowed.

---

## 2. Student Program Change (FR-DATA-3)

PRD says "Admin can manage student records (add, edit, link to program)." Only `CreateStudent` assigns a program; there is no way to change it.

| #   | Method | Route                        | Action                 | PRD Ref | Priority |
| --- | ------ | ---------------------------- | ---------------------- | ------- | -------- |
| 5   | `PUT`  | `/api/students/{id}/program` | Change student program | DATA-3  | **High** |

> **Why:** Students may need to transfer between programs; currently requires a DB patch.

---

## 3. Student Graduation (Manual) (FR-ENGINE-5 / FR-REPORT-3)

The control engine auto-identifies graduates, but there is no manual "mark as graduated" endpoint or graduation certificate generation.

| #   | Method | Route                                       | Action                          | PRD Ref  | Priority |
| --- | ------ | ------------------------------------------- | ------------------------------- | -------- | -------- |
| 6   | `PUT`  | `/api/students/{id}/graduate`               | Manually graduate a student     | ENGINE-5 | Medium   |
| 7   | `GET`  | `/api/students/{id}/graduation-certificate` | Download graduation certificate | REPORT-3 | Medium   |

> **Why:** Admin may need to override graduation status. Certificate download is listed as a requirement.

---

## 4. Student Current Load (Article 12)

`Student.GetMaxAllowedCreditHours()` already implements the rule, but no endpoint exposes it.

| #   | Method | Route                             | Action                              | PRD Ref | Priority |
| --- | ------ | --------------------------------- | ----------------------------------- | ------- | -------- |
| 8   | `GET`  | `/api/students/{id}/current-load` | Get enrolled vs max allowed credits | Art. 12 | **High** |

> **Why:** Registration UI needs to show the student their remaining capacity. Easy win — logic already exists in the domain.

---

## 5. Attendance & Deprivation (Article 19 / FR-GRADE)

PRD GRADE says the instructor must be able to mark a student as "Deprived." Currently no endpoint exists for this.

| #   | Method | Route                             | Action                                   | PRD Ref         | Priority |
| --- | ------ | --------------------------------- | ---------------------------------------- | --------------- | -------- |
| 9   | `PUT`  | `/api/registrations/{id}/deprive` | Mark student as deprived (absence > 25%) | GRADE / Art. 19 | Medium   |

> **Why:** Article 19 requires this; currently no way to record it.

---

## 6. Department Resource Browsing

No way to get a department's associated courses, faculty, or students in one call.

| #   | Method | Route                            | Action                          | PRD Ref | Priority |
| --- | ------ | -------------------------------- | ------------------------------- | ------- | -------- |
| 10  | `GET`  | `/api/departments/{id}/courses`  | Courses belonging to department | DATA-1  | Medium   |
| 11  | `GET`  | `/api/departments/{id}/faculty`  | Faculty members in department   | DATA-1  | Medium   |
| 12  | `GET`  | `/api/departments/{id}/students` | Students in department programs | DATA-1  | Medium   |

> **Why:** Admin dashboard needs department-level browsing.

---

## 7. Instructor Schedule

Instructor has no way to see their teaching schedule via API.

| #   | Method | Route                        | Action                                        | PRD Ref | Priority |
| --- | ------ | ---------------------------- | --------------------------------------------- | ------- | -------- |
| 13  | `GET`  | `/api/faculty/{id}/schedule` | Get instructor's course offerings by semester | —       | Medium   |

> **Why:** Instructors need to see what/where they teach. The `GET /api/faculty/{id}/courses` endpoint exists but returns course IDs only, not offering details.

---

## 8. Academic Reports (Enhancements)

| #   | Method | Route                             | Action                                                | PRD Ref | Priority |
| --- | ------ | --------------------------------- | ----------------------------------------------------- | ------- | -------- |
| 14  | `GET`  | `/api/reports/grade-distribution` | Grade distribution across all offerings in a semester | REPORT  | Low      |
| 15  | `GET`  | `/api/reports/semester-summary`   | Summary of registrations, pass rates per semester     | REPORT  | Low      |

> **Why:** Useful for admin dashboards but lower priority than core functionality.

---

## 9. System Configuration

| #   | Method | Route                       | Action                             | PRD Ref | Priority |
| --- | ------ | --------------------------- | ---------------------------------- | ------- | -------- |
| 16  | `PUT`  | `/api/config/grading-scale` | Override grading scale breakpoints | —       | Low      |
| 17  | `GET`  | `/api/config/grading-scale` | Get current grading scale          | Art. 27 | Low      |

> **Why:** The grading scale is currently hard-coded in `Grade.Create()`. Only needed if flexibility is required.

---

## Summary by Priority

| Priority  | Count  | Endpoints                                                                                                |
| --------- | ------ | -------------------------------------------------------------------------------------------------------- |
| **High**  | 5      | Registration period CRUD (4), Student program change, Current load                                       |
| Medium    | 6      | Graduate manually, Certificate download, Deprivation mark, Department resources (3), Instructor schedule |
| Low       | 4      | Reports (2), Grading scale config (2)                                                                    |
| **Total** | **15** |                                                                                                          |

---

## Quick Wins (already have domain logic, just need endpoints)

| #   | Endpoint                              | Domain Logic Exists At                         |
| --- | ------------------------------------- | ---------------------------------------------- |
| 8   | `GET /api/students/{id}/current-load` | `Student.cs:98` — `GetMaxAllowedCreditHours()` |
| 5   | `PUT /api/students/{id}/program`      | `Student` entity has `ProgramId` property      |
