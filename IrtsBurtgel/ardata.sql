INSERT INTO [dbo].[archived_meeting]
           ([meeting_id]
		   ,[name]
           ,[protocol]
           ,[meeting_datetime]
           ,[duration])
     VALUES
           (1,'TestArchivedMeeting1' , 'Protocol1', '20180823', '25');

INSERT INTO [dbo].[meeting_and_user]
           ([user_id]
           ,[meeting_id])
     VALUES
           (1, 1),
		   (2, 1);
		   
INSERT INTO [dbo].[status]
           ([name]
           ,[is_primary])
     VALUES
           (N'Ирсэн', 1),				/*1*/
           (N'Хоцорсон', 1),			/*2*/
           (N'Ээлжийн амралттай', 1),	/*3*/
           (N'Томилолттой', 1),			/*4*/
           (N'Гадаадад сургалттай', 1),	/*5*/
           (N'Ажлын хэсэгт', 1),		/*6*/
           (N'Өвчтэй', 1),				/*7*/
           (N'Чөлөөтэй', 1),			/*8*/
           (N'Сургуулийн чөлөөтэй', 1),	/*9*/
           (N'Жижүүрээс буусан', 1),	/*10*/
           (N'Ажилтай', 1),				/*11*/
           (N'Шөнө ажилласан', 1),		/*12*/
           (N'Бусад', 1),				/*13*/
           (N'Тасалсан', 1),			/*14*/
           (N'Тодорхойгүй', 1);			/*15*/

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
           ('2018-08-21 15:14', 3, 2, 2);







