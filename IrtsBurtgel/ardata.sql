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
           (N'Ирээгүй', 1);				/*15*/
use irtsBurtgel
INSERT INTO [dbo].[admin]
		   ([username]
		   ,[password]
		   ,[reg_date]
		   ,[organization_name]) 
	 VALUES
		   ('superadmin', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', CURRENT_TIMESTAMP, 'Bolorsoft LLC'); /*Password: password*/

INSERT INTO [dbo].[admin]
		   ([username]
		   ,[password]
		   ,[reg_date]
		   ,[organization_name]) 
	 VALUES
		   ('admin', '5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', CURRENT_TIMESTAMP, 'Police'); /*Password: password*/







