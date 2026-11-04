-- Capa, miolo, fibras e folhas originais; escrita é uma camada independente no jogo.
local root=assert(app.params.root)
local pc=app.pixelColor
local function color(r,g,b,a)return pc.rgba(r,g,b,a or 255)end
local function rect(im,x,y,w,h,c)for j=y,y+h-1 do for i=x,x+w-1 do if i>=0 and j>=0 and i<im.width and j<im.height then im:drawPixel(i,j,c) end end end end
local im=Image(640,352,ColorMode.RGB)
rect(im,13,6,614,338,color(37,31,27));rect(im,18,10,604,332,color(75,57,42));rect(im,21,12,598,326,color(96,73,51))
for y=15,332 do for x=24,615 do
 local side=x<320 and (320-x) or (x-320)
 local fold=math.max(0,1-side/28)*23
 local edge=math.min(x-24,615-x,y-15,332-y)<4 and 15 or 0
 local grain=((x*17+y*31+(x*y)%29)%7)-3
 local tone=grain-fold-edge
 im:drawPixel(x,y,color(218+tone,207+tone,177+tone))
end end
-- A lombada é sombreada; páginas amareladas têm borda e espessura próprias.
for y=15,332 do for x=314,325 do
 local v=math.abs(x-320)*7;im:drawPixel(x,y,color(100+v,82+v,59+v))
end end
for y=335,339,2 do rect(im,26,y,285,1,color(166,147,115));rect(im,329,y,286,1,color(166,147,115))end
for x=26,610,17 do im:drawPixel(x,8,color(132,105,76));im:drawPixel(x,343,color(132,105,76))end
-- Marcas suaves de manuseio, fora da área de leitura.
for i=1,1500 do
 local x=(i*97)%590+25;local y=(i*53)%315+16
 if x<44 or x>595 or y<25 or y>322 then im:drawPixel(x,y,color(185+(i%9),170+(i%9),140+(i%9)))end
end
for y=309,331 do for x=593,615 do if x+y>919 then im:drawPixel(x,y,color(190,176,143))end end end
local spr=Sprite(640,352,ColorMode.RGB);spr.cels[1].image=im;spr:saveAs(root..'/Caderno.aseprite');spr:saveCopyAs(root..'/Book.png');spr:close()
local leaf=Image(278,288,ColorMode.RGB)
for y=0,287 do for x=0,277 do local g=((x*13+y*19)%5)-2;leaf:drawPixel(x,y,color(218+g,207+g,177+g))end end
local s=Sprite(278,288,ColorMode.RGB);s.cels[1].image=leaf;s:saveAs(root..'/Folha.aseprite');s:saveCopyAs(root..'/Leaf.png');s:close()
print('Caderno e folha exportados, sem escrita embutida na textura.')
local mark=Image(40,30,ColorMode.RGB)
for i=0,200 do
 local a=i*math.pi*2/200;local x=math.floor(20+17*math.cos(a)+.7*math.sin(a*7));local y=math.floor(14+10*math.sin(a)+.5*math.cos(a*5))
 mark:drawPixel(x,y,color(144,74,51,220));if i%5==0 then mark:drawPixel(x,y+1,color(144,74,51,110))end
end
local m=Sprite(40,30,ColorMode.RGB);m.cels[1].image=mark;m:saveAs(root..'/Rabisco.aseprite');m:saveCopyAs(root..'/Sketch.png');m:close()
