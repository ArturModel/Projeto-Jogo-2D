local root=assert(app.params.root);math.randomseed(200436)
local s=Sprite(128,128,ColorMode.RGB);local im=Image(128,128,ColorMode.RGB)
local function rgba(r,g,b)return app.pixelColor.rgba(r,g,b,255)end
local function box(x,y,w,h,tone)
 for py=y,math.min(111,y+h)do for px=x,x+w do
  local n=math.random(-3,3);local edge=px==x or px==x+w or py==y or py==y+h
  local side=px>x+w*.78 and -17 or 0
  im:drawPixel(px,py,edge and rgba(40,32,24) or rgba(tone+n+side,math.floor(tone*.72)+n+side,math.floor(tone*.43)+n+side))
 end end
 for py=y+1,y+h-1 do for px=x+math.floor(w*.42),x+math.floor(w*.48)do im:drawPixel(px,py,rgba(148,128,87))end end
 for px=x+5,x+math.floor(w*.28)do im:drawPixel(px,y+math.floor(h*.65),rgba(64,52,35))end
 for px=x+5,x+math.floor(w*.23)do im:drawPixel(px,y+math.floor(h*.65)+3,rgba(64,52,35))end
end
box(12,77,58,34,120);box(70,74,43,37,107)
box(19,43,47,33,131);box(67,40,37,33,117)
box(27,18,62,24,126)
-- Etiquetas de conferência e fita de embalagem, sem marca inventada.
for y=87,97 do for x=25,41 do im:drawPixel(x,y,rgba(176,166,130))end end
for x=28,38 do im:drawPixel(x,90,rgba(66,63,48));im:drawPixel(x,94,rgba(66,63,48))end
s.cels[1].image=im;s:saveAs(root..'/CargasMercado.aseprite');s:saveCopyAs(root..'/CargasMercado.png');s:close()
print('Estoque de mercadoria em caixas fechadas, sem ferramentas de oficina.')
local book=Sprite(128,128,ColorMode.RGB);local page=Image(128,128,ColorMode.RGB)
for y=59,111 do for x=23,104 do
 local border=x<27 or x>101 or y<62 or y>108;local n=math.random(-2,2)
 page:drawPixel(x,y,border and rgba(28,28,23) or x<33 and rgba(57,58,40) or rgba(100+n,96+n,63+n))
end end
for y=105,108 do for x=34,100 do page:drawPixel(x,y,rgba(177,165,127))end end
for y=72,90 do for x=46,86 do page:drawPixel(x,y,rgba(178,168,132))end end
for _,y in ipairs({77,82,87})do for x=52,79 do page:drawPixel(x,y,rgba(82,77,58))end end
book.cels[1].image=page;book:saveAs(root..'/Caderno.aseprite');book:saveCopyAs(root..'/Caderno.png');book:close()
