-- Native-scale additions, using the existing bar/Marcus palette; no 3D render baking.
local root=assert(app.params.root);local pc=app.pixelColor
local function c(r,g,b,a)return pc.rgba(r,g,b,a or 255)end
local dark=c(35,29,23);local wood=c(101,70,42);local light=c(171,132,77);local paper=c(196,183,139)
local function box(im,x,y,w,h,col)for yy=y,y+h-1 do for xx=x,x+w-1 do im:drawPixel(xx,yy,col)end end end
local function save(im,n)local s=Sprite(im.width,im.height,ColorMode.RGB);s.cels[1].image=im;s.layers[1].name='Pixel art 2D / '..n;s:saveAs(root..'/Arte/Producao/BarIdentidade2D/'..n..'.aseprite');s:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Props/'..n..'.png');s:close()end
local im=Image(36,44,ColorMode.RGB);box(im,1,1,34,41,dark);box(im,3,3,30,37,wood);box(im,5,5,26,33,c(42,48,38));box(im,6,4,24,1,light)
-- Small painted letter clusters: full text is readable in examination, not stretched raster text.
for row=0,2 do for n=0,(row==2 and 5 or 4)do local x=7+n*4;local y=11+row*8;box(im,x,y,2,5,paper);box(im,x+1,y+1,2,1,paper);box(im,x+1,y+3,2,1,paper)end end
save(im,'FiadoBar')
im=Image(40,32,ColorMode.RGB);box(im,1,1,38,29,dark);box(im,3,3,34,25,light);box(im,5,5,30,21,c(116,118,86));box(im,5,18,30,8,c(64,75,48))
for i=0,6 do local x=7+i*4;box(im,x,10,3,3,c(183,147,101));box(im,x,13,3,5,c(131,55,38));box(im,x,18,1,5,dark);box(im,x+2,18,1,5,dark)end
save(im,'FotoTimeBar')
im=Image(44,64,ColorMode.RGB);box(im,2,54,40,5,dark);box(im,3,53,38,3,wood);box(im,2,12,40,4,wood)
for i=0,5 do local x=6+i*6;box(im,x,8,1,47,c(186,152,93));box(im,x,8,1,3,c(86,125,125));box(im,x+1,21,1,32,c(121,87,49))end
save(im,'TaqueiraBar')
im=Image(40,38,ColorMode.RGB);box(im,2,14,36,20,c(101,38,29));box(im,4,16,32,2,c(152,65,41));box(im,3,31,34,2,dark)
for i=0,5 do local x=5+i*5;box(im,x,4,3,17,c(56,66,34));box(im,x+1,2,1,6,c(78,86,48));box(im,x,24,3,5,dark)end
save(im,'EngradadoBar')
im=Image(112,60,ColorMode.RGB)
for y=3,56 do for x=3,108 do local edge=math.min(x-3,108-x,y-3,56-y);local col=c(67,48,36);if edge<2 or edge==5 then col=c(129,105,65)elseif edge<7 then col=c(42,47,40)elseif (math.abs(x%12-6)+math.abs(y%12-6))<3 then col=c(117,75,47)end;im:drawPixel(x,y,col)end end
save(im,'TapeteBar')
im=Image(48,40,ColorMode.RGB);box(im,1,3,46,32,c(132,35,32));box(im,2,4,44,30,c(159,42,36));box(im,2,4,1,30,c(188,57,46));box(im,46,4,1,30,dark)
local src=Sprite{fromFile=root..'/Projeto Jogo 2D/Assets/Resources/Art/BarRef7/Inter1993.png'}
local cel=src.cels[1]
for y=0,19 do for x=0,19 do if (x-9.5)^2+(y-9.5)^2<=100 then local sx=568+math.floor((x+.5)*264/20)-cel.position.x;local sy=383+math.floor((y+.5)*264/20)-cel.position.y;local raw=cel.image:getPixel(sx,sy);if src.colorMode==ColorMode.INDEXED then local col=src.palettes[1]:getColor(raw);raw=c(col.red,col.green,col.blue,col.alpha)end;im:drawPixel(14+x,13+y,raw)end end end
src:close()
for n=0,3 do local x=16+n*5;box(im,x,8,1,3,c(211,184,93));box(im,x-1,9,3,1,c(211,184,93))end
save(im,'Inter1994Bar')
print('Six native 2D additions exported with editable sources.')

im=Image(28,36,ColorMode.RGB)
box(im,5,11,3,23,dark);box(im,20,11,3,23,dark);box(im,6,13,2,20,wood);box(im,20,13,2,20,wood);box(im,6,26,16,3,dark);box(im,7,26,14,1,light)
for y=5,13 do for x=2,25 do if ((x-14)/12)^2+((y-9)/5)^2<=1 then im:drawPixel(x,y,y<9 and light or wood)end end end
box(im,5,7,17,1,c(187,148,93));save(im,'BancoBar')

im=Image(32,32,ColorMode.RGB)
box(im,3,5,9,20,dark);box(im,4,5,7,19,c(57,83,65));box(im,5,7,2,15,c(95,119,95));box(im,4,3,7,3,c(45,56,46));box(im,11,9,3,9,c(35,43,33));box(im,12,10,1,7,c(134,130,98))
for y=18,27 do for x=18,28 do if ((x-23)/6)^2+((y-22)/6)^2<1 then im:drawPixel(x,y,c(104,65,38))end end end
box(im,18,18,10,3,c(86,102,53));box(im,24,10,1,11,c(166,167,142));box(im,24,9,4,1,c(193,192,165));box(im,20,26,6,1,light)
save(im,'CuiaBar')

-- Detailed examination artwork; same historical emblem, clean RGBA rather than montage.
im=Image(256,256,ColorMode.RGB);box(im,0,0,256,256,c(153,31,28))
local src=Sprite{fromFile=root..'/Projeto Jogo 2D/Assets/Resources/Art/BarRef7/Inter1993.png'};local cel=src.cels[1]
for y=0,175 do for x=0,175 do if (x-87.5)^2+(y-87.5)^2<=88^2 then local raw=cel.image:getPixel(568+math.floor(x*264/176)-cel.position.x,383+math.floor(y*264/176)-cel.position.y);if src.colorMode==ColorMode.INDEXED then local col=src.palettes[1]:getColor(raw);raw=c(col.red,col.green,col.blue,col.alpha)end;im:drawPixel(40+x,60+y,raw)end end end;src:close()
for n=0,3 do local cx=74+n*36;for y=-8,8 do for x=-8,8 do local ang=math.atan(y,x);if x*x+y*y<=(5.5+2.5*math.cos(5*(ang+math.pi/2)))^2 then im:drawPixel(cx+x,30+y,c(224,198,97))end end end end
save(im,'Inter1994Detalhe')
