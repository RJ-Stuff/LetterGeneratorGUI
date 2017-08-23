select  DISTINCT 
	lc.cod_luna as codluna,
	LTRIM(RTRIM(UPPER(l.NOMBRE))) + ' ' + LTRIM(RTRIM(UPPER(APELLIDO_PATERNO)))+ ' '+ LTRIM(RTRIM(UPPER(APELLIDO_MATERNO))) as clientname,
	lc.exigible as totaldebt,
	l.NRO_IDENTIFICACION as docid,
	null as bill,
	CASE s.COD_SISTEMA
		WHEN 1 THEN 'FIJA'
		WHEN 2 THEN 'CABLE'
		WHEN 3 THEN 'MÓVIL'
		WHEN 4 THEN 'AMDOCS'
	END as service,
	s.Telefono as phonenumber,
	fecha_vencimiento as duedate,
	SUM(MONTO_EXIGIBLE) over (partition by d.cod_sistema, d.cod_cliente, d.cod_cuenta, d.fecha_vencimiento) as debt,
	'TDP' as business,
	r.descripcion as duerange,
	z.NOMBRE as zonal,
	m.ultimadireccionubicada as alternativeaddress, 
	m.ultimadireccionubicada2, 
	m.ultimadireccionporverificar as newaddress, 
	m.ultimadireccionporverificar2,
	null as baseaddress,
	null as sector,
	null as district,
	'CAMPO' as managementkind 
from luna_control lc
	inner join master_luna l on l.cod_luna = lc.cod_luna
	inner join core_zonal z on z.COD_ZONAL = l.COD_ZONAL
	--filtros obligatorio (éste cambiaría cada vez que algún jefe quiera cambiar de estrategia)
	inner join core_rango_deuda r on l.cod_rango_deuda_avanzada = r.cod_rango_deuda and r.cod_rango_deuda in (5,6)
	inner join master_cliente c on c.cod_luna = l.cod_luna
	inner join master_servicio s on s.cod_sistema = c.cod_sistema and s.cod_cliente = c.cod_cliente and s.activo = 1
	inner join tran_deuda d on d.cod_sistema = s.cod_sistema and d.cod_cliente = s.cod_cliente and d.cod_cuenta = s.cod_cuenta and d.flag_retiro = 0
	left join master_cuenta_indicadores i on i.cod_sistema = d.cod_sistema and i.cod_cliente = d.cod_cliente and i.COD_CUENTA = d.COD_CUENTA and i.COD_INDICADOR= 8
	inner join moroso m on lc.moroso = m.moroso
where 
	--filtros obligatorios
	lc.exigible > 400 
	and lc.retiro = 0 
	and lc.estadogestioncall = 3 
	and i.cod_indicador is null

	and r.cod_rango_deuda = 6

	and z.NOMBRE like'%tru%'
	--filtros seleccionados dentro de la aplicación
	--CUSTOMFILTERS



