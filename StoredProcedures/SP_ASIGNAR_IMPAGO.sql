USE [SubscriptionDb]
GO
/****** Object:  StoredProcedure [dbo].[SP_ASIGNAR_IMPAGO]    Script Date: 14/9/2023 13:03:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_ASIGNAR_IMPAGO] 
AS
BEGIN
	UPDATE AspNetUsers SET Impago = 1
	FROM Facturas AS f
	INNER JOIN AspNetUsers u
	ON f.UsuarioId = u.Id
	WHERE Pagada = 0 
		AND GETDATE() > f.FechaLimitePago
END
