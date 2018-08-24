INSERT INTO [dbo].[archived_meeting]
           ([m_meeting_id]
           ,[meeting_id]
           ,[protocol]
           ,[meeting_datetime]
           ,[duration])
     VALUES
           (1, 1, 'Protocol1', '20180823', '25');

INSERT INTO [dbo].[meeting_and_user]
           ([user_id]
           ,[meeting_id])
     VALUES
           (1, 1),
		   (2, 1);

INSERT INTO [dbo].[user_status]
           ([start_date]
           ,[end_date]
           ,[user_id]
           ,[status_id]
           ,[is_deleted])
     VALUES
           ('20180821', '20180824', 3, 2, 0);

INSERT INTO [dbo].[attendance]
           ([reg_time]
           ,[a_meeting_id]
           ,[user_id]
           ,[status_id])
     VALUES
           ('2018-08-21 15:14', 2, 2, 2);

INSERT INTO [dbo].[status]
           ([name]
           ,[is_primary])
     VALUES
           ('Ирсэн', 1),
           ('Ээлжийн амралттай', 1),
           ('Томилолттой', 1),
           ('Гадаадад сургалттай', 1),
           ('Ажлын хэсэгт', 1),
           ('Өвчтэй', 1),
           ('Чөлөөтэй', 1),
           ('Сургуулийн чөлөөтэй', 1),
           ('Жижүүрээс буусан', 1),
           ('Ажилтай', 1),
           ('Шөнө ажилласан', 1),
           ('Бусад', 1),
           ('Тасалсан', 1),
           ('Тодорхойгүй', 1);






