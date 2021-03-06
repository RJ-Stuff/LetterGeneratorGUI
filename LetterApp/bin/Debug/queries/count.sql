select 
	count(distinct xy1.Direccion)
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