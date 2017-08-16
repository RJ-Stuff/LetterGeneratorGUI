:: Autor: Rigoberto Leander Salgado Reyes <rigoberto.salgado@rjabogados.com>
::
:: © 2017 RJAbogados
:: Todos los derechos reservados.
::

@echo off

latex\install\miktex\bin\latex.exe -output-directory=temp --src --interaction=errorstopmode --synctex=-1 "%1.tex"
dvipdfmx.exe "%1.dvi" -o %2
