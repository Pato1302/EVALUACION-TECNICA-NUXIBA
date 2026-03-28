-- Consulta del usuario que más tiempo ha estado logueado
WITH
    Sesiones
    AS
    (
        SELECT
            l.User_id,
            l.fecha AS LoginTime,
            (
            SELECT MIN(o.fecha)
            FROM ccloglogin o
            WHERE o.User_id = l.User_id
                AND o.TipoMov = 0
                AND o.fecha > l.fecha
        ) AS LogoutTime
        FROM ccloglogin l
        WHERE l.TipoMov = 1
    ),
    Tiempos
    AS
    (
        SELECT
            User_id,
            DATEDIFF(SECOND, LoginTime, LogoutTime) AS DuracionSegundos
        FROM Sesiones
        WHERE LogoutTime IS NOT NULL
    ),
    Totales
    AS
    (
        SELECT
            User_id,
            SUM(DuracionSegundos) AS TiempoTotalSegundos
        FROM Tiempos
        GROUP BY User_id
    )
SELECT TOP 1
    User_id,
    TiempoTotalSegundos / 86400 AS Dias,
    (TiempoTotalSegundos % 86400) / 3600 AS Horas,
    (TiempoTotalSegundos % 3600) / 60 AS Minutos,
    TiempoTotalSegundos % 60 AS Segundos
FROM Totales
ORDER BY TiempoTotalSegundos DESC;

-- Consulta del usuario que menos tiempo ha estado logueado
WITH
    Sesiones
    AS
    (
        SELECT
            l.User_id,
            l.fecha AS LoginTime,
            (
            SELECT MIN(o.fecha)
            FROM ccloglogin o
            WHERE o.User_id = l.User_id
                AND o.TipoMov = 0
                AND o.fecha > l.fecha
        ) AS LogoutTime
        FROM ccloglogin l
        WHERE l.TipoMov = 1
    ),
    Tiempos
    AS
    (
        SELECT
            User_id,
            DATEDIFF(SECOND, LoginTime, LogoutTime) AS DuracionSegundos
        FROM Sesiones
        WHERE LogoutTime IS NOT NULL
    ),
    Totales
    AS
    (
        SELECT
            User_id,
            SUM(DuracionSegundos) AS TiempoTotalSegundos
        FROM Tiempos
        GROUP BY User_id
    )
SELECT TOP 1
    User_id,
    TiempoTotalSegundos / 86400 AS Dias,
    (TiempoTotalSegundos % 86400) / 3600 AS Horas,
    (TiempoTotalSegundos % 3600) / 60 AS Minutos,
    TiempoTotalSegundos % 60 AS Segundos
FROM Totales
ORDER BY TiempoTotalSegundos ASC;

-- Promedio de logueo por mes
WITH
    Sesiones
    AS
    (
        SELECT
            l.User_id,
            l.fecha AS LoginTime,
            (
            SELECT MIN(o.fecha)
            FROM ccloglogin o
            WHERE o.User_id = l.User_id
                AND o.TipoMov = 0
                AND o.fecha > l.fecha
        ) AS LogoutTime
        FROM ccloglogin l
        WHERE l.TipoMov = 1
    ),
    Tiempos
    AS
    (
        SELECT
            User_id,
            DATEPART(YEAR, LoginTime) AS Año,
            DATEPART(MONTH, LoginTime) AS Mes,
            DATEDIFF(SECOND, LoginTime, LogoutTime) AS DuracionSegundos
        FROM Sesiones
        WHERE LogoutTime IS NOT NULL
    ),
    Promedios
    AS
    (
        SELECT
            User_id,
            Año,
            Mes,
            AVG(DuracionSegundos) AS PromedioSegundos
        FROM Tiempos
        GROUP BY User_id, Año, Mes
    )
SELECT
    User_id,
    Año,
    Mes,
    PromedioSegundos / 86400 AS Dias,
    (PromedioSegundos % 86400) / 3600 AS Horas,
    (PromedioSegundos % 3600) / 60 AS Minutos,
    PromedioSegundos % 60 AS Segundos
FROM Promedios
ORDER BY User_id, Año, Mes;