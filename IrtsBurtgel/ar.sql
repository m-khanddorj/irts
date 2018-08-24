
CREATE TABLE "position" (
                position_id INT IDENTITY NOT NULL,
                name VARCHAR(255) NOT NULL,
                is_deleted BIT DEFAULT 0 NOT NULL,
                CONSTRAINT position_id PRIMARY KEY (position_id)
)

CREATE TABLE department (
                department_id INT IDENTITY NOT NULL,
                name VARCHAR(255) NOT NULL,
                is_deleted BIT DEFAULT 0 NOT NULL,
                CONSTRAINT department_id PRIMARY KEY (department_id)
)

CREATE TABLE admin (
                admin_id INT IDENTITY NOT NULL,
                username VARCHAR(50) NOT NULL,
                password VARCHAR(255) NOT NULL,
                reg_date DATETIME NOT NULL,
                organization_name VARCHAR(255) NOT NULL,
                CONSTRAINT admin_id PRIMARY KEY (admin_id)
)

CREATE TABLE "user" (
                user_id INT IDENTITY NOT NULL,
                pin INT NOT NULL,
                fname NVARCHAR(50) NOT NULL,
                lname NVARCHAR(50) NOT NULL,
                fingerprint_0 TEXT NOT NULL,
                fingerprint_1 TEXT NOT NULL,
                is_deleted BIT DEFAULT 0 NOT NULL,
                department_id INT,
                position_id INT,
                CONSTRAINT user_id PRIMARY KEY (user_id)
)

CREATE TABLE event (
                event_id INT IDENTITY NOT NULL,
                name VARCHAR(255) DEFAULT 50 NOT NULL,
                start_date DATETIME NOT NULL,
                end_date DATETIME NOT NULL,
                interval_type TINYINT NOT NULL,
                interval_day INT,
                week TINYINT,
                is_deleted BIT DEFAULT 0 NOT NULL,
                CONSTRAINT event_id PRIMARY KEY (event_id)
)

CREATE TABLE status (
                status_id INT IDENTITY NOT NULL,
                name VARCHAR(50) NOT NULL,
                is_primary BIT DEFAULT 0 NOT NULL,
                CONSTRAINT status_id PRIMARY KEY (status_id)
)

CREATE TABLE user_status (
                user_status_id INT IDENTITY NOT NULL,
                start_date DATETIME NOT NULL,
                end_date DATETIME NOT NULL,
                user_id INT NOT NULL,
                status_id INT NOT NULL,
                is_deleted BIT DEFAULT 0 NOT NULL,
                CONSTRAINT user_status_id PRIMARY KEY (user_status_id)
)

CREATE TABLE meeting (
                meeting_id INT IDENTITY NOT NULL,
                start_datetime DATETIME NOT NULL,
                duration INT NOT NULL,
                name VARCHAR(255) NOT NULL,
                is_deleted BIT DEFAULT 0 NOT NULL,
                end_date DATETIME,
                interval_type TINYINT DEFAULT 0 NOT NULL,
                interval_day INT DEFAULT 0,
                week TINYINT DEFAULT 0,
                CONSTRAINT meeting_id PRIMARY KEY (meeting_id)
)

CREATE TABLE meeting_and_position (
                mpid INT IDENTITY NOT NULL,
                position_id INT NOT NULL,
                meeting_id INT NOT NULL,
                CONSTRAINT mpid PRIMARY KEY (mpid)
)

CREATE TABLE meeting_and_department (
                mdid INT IDENTITY NOT NULL,
                department_id INT NOT NULL,
                meeting_id INT NOT NULL,
                CONSTRAINT mdid PRIMARY KEY (mdid)
)

CREATE TABLE modified_meeting (
                m_meeting_id INT IDENTITY NOT NULL,
                name VARCHAR(255) NOT NULL,
                start_datetime DATETIME NOT NULL,
                duration INT NOT NULL,
                is_deleted BIT DEFAULT 0 NOT NULL,
                meeting_order INT DEFAULT 0 NOT NULL,
                reason VARCHAR(255),
                event_id INT,
                meeting_id INT NOT NULL,
                CONSTRAINT m_meeting_id PRIMARY KEY (m_meeting_id)
)

CREATE TABLE archived_meeting (
                a_meeting_id INT IDENTITY NOT NULL,
                m_meeting_id INT,
                meeting_id INT NOT NULL,
                protocol VARCHAR(255),
                meeting_datetime DATETIME NOT NULL,
                duration INT NOT NULL,
                name VARCHAR(255) NOT NULL,
                CONSTRAINT a_meeting_id PRIMARY KEY (a_meeting_id)
)

CREATE TABLE attendance (
                attendance_id INT IDENTITY NOT NULL,
                user_id INT NOT NULL,
                status_id INT NOT NULL,
                a_meeting_id INT NOT NULL,
                reg_time DATETIME NOT NULL,
                CONSTRAINT attendance_id PRIMARY KEY (attendance_id)
)

CREATE TABLE meeting_and_user (
                muid INT IDENTITY NOT NULL,
                user_id INT NOT NULL,
                meeting_id INT NOT NULL,
                CONSTRAINT muid PRIMARY KEY (muid)
)

ALTER TABLE "user" ADD CONSTRAINT _position___user__fk
FOREIGN KEY (position_id)
REFERENCES "position" (position_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE meeting_and_position ADD CONSTRAINT _position__meeting_and_position_fk
FOREIGN KEY (position_id)
REFERENCES "position" (position_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE "user" ADD CONSTRAINT department__user__fk
FOREIGN KEY (department_id)
REFERENCES department (department_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE meeting_and_department ADD CONSTRAINT department_meeting_and_department_fk
FOREIGN KEY (department_id)
REFERENCES department (department_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE meeting_and_user ADD CONSTRAINT user_meeting_and_user_fk
FOREIGN KEY (user_id)
REFERENCES "user" (user_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE attendance ADD CONSTRAINT user_attendance_fk
FOREIGN KEY (user_id)
REFERENCES "user" (user_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE user_status ADD CONSTRAINT _user__user_status_fk
FOREIGN KEY (user_id)
REFERENCES "user" (user_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE modified_meeting ADD CONSTRAINT event_modified_meeting_fk
FOREIGN KEY (event_id)
REFERENCES event (event_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE attendance ADD CONSTRAINT status_attendance_fk
FOREIGN KEY (status_id)
REFERENCES status (status_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE user_status ADD CONSTRAINT status_user_status_fk
FOREIGN KEY (status_id)
REFERENCES status (status_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE meeting_and_user ADD CONSTRAINT meeting_meeting_and_user_fk
FOREIGN KEY (meeting_id)
REFERENCES meeting (meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE modified_meeting ADD CONSTRAINT meeting_modified_meeting_fk
FOREIGN KEY (meeting_id)
REFERENCES meeting (meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE archived_meeting ADD CONSTRAINT meeting_archived_meeting_fk
FOREIGN KEY (meeting_id)
REFERENCES meeting (meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE meeting_and_department ADD CONSTRAINT meeting_meeting_and_department_fk
FOREIGN KEY (meeting_id)
REFERENCES meeting (meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE meeting_and_position ADD CONSTRAINT meeting_meeting_and_position_fk
FOREIGN KEY (meeting_id)
REFERENCES meeting (meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE archived_meeting ADD CONSTRAINT modified_meeting_archived_meeting_fk
FOREIGN KEY (m_meeting_id)
REFERENCES modified_meeting (m_meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION

ALTER TABLE attendance ADD CONSTRAINT archived_meeting_attendance_fk
FOREIGN KEY (a_meeting_id)
REFERENCES archived_meeting (a_meeting_id)
ON DELETE NO ACTION
ON UPDATE NO ACTION
