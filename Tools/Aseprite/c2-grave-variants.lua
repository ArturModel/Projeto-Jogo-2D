-- Conserva pedra, perspectiva e base do asset do projeto; varia só monumentos comuns.
local src=app.open(assert(app.params.source));local original=src.cels[1].image;local root=assert(app.params.root)
for variant=1,2 do
 local im=Image(original)
 for y=0,51 do for x=43,85 do im:drawPixel(x,y,0) end end
 if variant==1 then
  -- Cabeceira baixa: a mesma pedra, com altura reduzida e base apoiada no mesmo ponto.
  for y=0,20 do for x=43,85 do im:drawPixel(x,32+y,original:getPixel(x,10+y*2)) end end
 else
  -- Lápide horizontal rasa, sem monumento vertical. Não altera o jazigo da família.
  local pc=app.pixelColor
  for y=44,53 do for x=49,80 do
   local col=pc.rgba(94,91,76,255)
   if y==44 or x==49 then col=pc.rgba(132,125,101,255) end
   if y==53 or x==80 then col=pc.rgba(57,56,48,255) end
   if y>46 and y<51 and x>54 and x<74 and (y%2==1) then col=pc.rgba(65,65,56,255) end
   im:drawPixel(x,y,col)
  end end
 end
 local name=variant==1 and 'SepulturaBaixa' or 'SepulturaHorizontal'
 local s=Sprite(128,128,ColorMode.RGB);s.cels[1].image=im;s:saveAs(root..'/'..name..'.aseprite');s:saveCopyAs(root..'/'..name..'.png');s:close()
end
src:close();print('Duas variantes de sepulturas comuns; jazigo canônico preservado.')
