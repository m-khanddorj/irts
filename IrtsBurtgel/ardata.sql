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
		   ('superadmin', 'f368f16e52664d1849cee26464ed55bb40822447400e928d7a4c93804096c367', CURRENT_TIMESTAMP, 'Bolorsoft LLC'); /*Password: password*/

INSERT INTO [dbo].[admin]
		   ([username]
		   ,[password]
		   ,[reg_date]
		   ,[organization_name]) 
	 VALUES
		   ('admin', 'f368f16e52664d1849cee26464ed55bb40822447400e928d7a4c93804096c367', CURRENT_TIMESTAMP, 'Police'); /*Password: password*/







