USE [SubscriptionDb]
GO
/****** Object:  StoredProcedure [dbo].[SP_CREACION_FACTURAS]    Script Date: 14/9/2023 13:04:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[SP_CREACION_FACTURAS]
	@FechaInicio DATETIME,
	@FechaFin DATETIME
AS
BEGIN
	DECLARE @MontoPorPeticion DECIMAL(4, 4) = 1.0/2 -- 2.0/1000 -- 2 dolares por 1000 peticiones

	INSERT INTO Facturas 
		(UsuarioId, Monto, Pagada, FechaEmision, FechaLimitePago)
	SELECT la.UsuarioId, 
		COUNT(1) * @MontoPorPeticion AS Monto,
		0 AS Pagada,
		GETDATE() AS FechaEmision,
		DATEADD(DAY, 60, GETDATE()) FechaLimitePago
	FROM Peticiones p
	INNER JOIN LlavesApi la
		ON p.LlaveId = la.Id
	WHERE la.TipoLlave != 1
		AND p.FechaPeticion >= @FechaInicio 
		AND p.FechaPeticion < @FechaFin
	GROUP BY la.UsuarioId

	INSERT INTO FacturasEmitidas (Mes, Anio)
	SELECT 
		CASE MONTH(GETDATE())
			WHEN 1 THEN 12 
			ELSE MONTH(GETDATE()) - 1 
		END AS Mes,
		CASE MONTH(GETDATE()) 
			WHEN 1 THEN YEAR(GETDATE()) - 1 
			ELSE YEAR(GETDATE())
		END AS Anio
END
