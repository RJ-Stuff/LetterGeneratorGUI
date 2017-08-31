select DISTINCT 
	lc.cod_luna as Código_Cliente,
    LTRIM(RTRIM(UPPER(l.NOMBRE))) + ' ' + LTRIM(RTRIM(UPPER(APELLIDO_PATERNO)))+ ' '+ LTRIM(RTRIM(UPPER(APELLIDO_MATERNO))) as Nombre_Cliente,
    lc.exigible as Deuda_Total,
    l.NRO_IDENTIFICACION as No_Identificación,
    null as Factura,
    CASE s.COD_SISTEMA
    WHEN 1 THEN 'FIJA'
    WHEN 2 THEN 'CABLE'
    WHEN 3 THEN 'MÓVIL'
    WHEN 4 THEN 'AMDOCS'
    END as Servicio,
    s.Telefono as Número_Teléfono,
    fecha_vencimiento as Fecha_Vencimiento,
    SUM(MONTO_EXIGIBLE) over (partition by d.cod_sistema, d.cod_cliente, d.cod_cuenta, d.fecha_vencimiento, xy1.direccion) as Deuda,
    'TDP' as Negocio,
    r.descripcion as Rango_Deuda,
    z.NOMBRE as Zonal,
    Dirección_Base_Carta = xy1.Direccion,
    Dirrecion_Base = xy.Direccion,
    Departamento_Base = xy.Departamento,
    Direccion_Ubicada1 = m.ultimadireccionubicada,
    Direccion_Ubicada2 = m.UltimaDireccionUbicada2,
    Direccion_Nueva1 = m.UltimaDireccionPorVerificar,
    Direccion_Nueva2 = m.UltimaDireccionPorVerificar2,
	null as Sector,
	null as Distrito,
	'CAMPO' as Tipo_Getión,
	DATEDIFF(day, fecha_vencimiento, getdate()) as Días_Mora
from luna_control lc
	inner join moroso m on lc.moroso = m.moroso
	inner join master_luna l on l.cod_luna = lc.cod_luna
	inner join core_zonal z on z.COD_ZONAL = l.COD_ZONAL
	inner join core_rango_deuda r on l.cod_rango_deuda_avanzada = r.cod_rango_deuda and r.cod_rango_deuda in (5,6)
	inner join master_cliente c on c.cod_luna = l.cod_luna
	inner join master_servicio s on s.cod_sistema = c.cod_sistema and s.cod_cliente = c.cod_cliente and s.activo = 1
	inner join tran_deuda d on d.cod_sistema = s.cod_sistema and d.cod_cliente = s.cod_cliente and d.cod_cuenta = s.cod_cuenta and d.flag_retiro = 0
	left join master_direcciones_x_y xy on xy.cod_sistema = s.cod_sistema and xy.cod_cliente = s.cod_cliente and xy.cod_cuenta = s.cod_cuenta
	inner join master_direcciones_x_y xy1 on xy1.cod_luna = lc.cod_luna
	left join master_cuenta_indicadores i on i.cod_sistema = d.cod_sistema and i.cod_cliente = d.cod_cliente and i.COD_CUENTA = d.COD_CUENTA and i.COD_INDICADOR= 8
where 
	lc.exigible > 400 
	and lc.retiro = 0 
	and lc.estadogestioncall = 3 
	and i.cod_indicador is null
	and d.fecha_vencimiento < getdate()

	--CUSTOMFILTERS
