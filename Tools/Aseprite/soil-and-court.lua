local root=assert(app.params.root);local tiles=assert(app.params.tiles);math.randomseed(200409)
local function rgb(r,g,b)return app.pixelColor.rgba(r,g,b,255)end
local image=Image(128,128,ColorMode.RGB)
for y=0,127 do for x=0,127 do
  local wave=math.sin(x*math.pi/32)*math.cos(y*math.pi/64)*2;local n=math.random(-3,3)+wave
  image:drawPixel(x,y,rgb(80+n,64+n,44+n))
end end
for i=1,650 do local x,y=math.random(0,127),math.random(0,127);local n=math.random(0,8);image:drawPixel(x,y,rgb(89+n,75+n,55+n)) end
local macro=Sprite(128,128,ColorMode.RGB);macro.cels[1].image=image;macro:saveAs(root..'/Terra_Macro.aseprite');macro:saveCopyAs(root..'/Terra_Macro.png');macro:close()
for y=0,3 do for x=0,3 do
 local s=Sprite(32,32,ColorMode.RGB);local t=Image(32,32,ColorMode.RGB)
 for py=0,31 do for px=0,31 do t:drawPixel(px,py,image:getPixel(x*32+px,y*32+py)) end end
 s.cels[1].image=t;s:saveCopyAs(root..'/Terra_'..x..'_'..(3-y)..'.png');s:close()
end end
local sand=Sprite(32,32,ColorMode.RGB);local im=Image(32,32,ColorMode.RGB)
for y=0,31 do for x=0,31 do local n=math.random(-3,3);im:drawPixel(x,y,rgb(126+n,106+n,74+n)) end end
sand.cels[1].image=im;sand:saveAs(tiles..'/Areia.aseprite');sand:saveCopyAs(tiles..'/Areia.png');sand:close()
local rail=Sprite(128,10,ColorMode.RGB);local wood=Image(128,10,ColorMode.RGB)
for y=0,9 do for x=0,127 do local n=math.random(-3,3);local border=y==0 or y==9;wood:drawPixel(x,y,border and rgb(38,29,20) or rgb(89+n+(y==2 and 14 or 0),62+n,36+n)) end end
rail.cels[1].image=wood;rail:saveAs(root..'/CanchaGuia.aseprite');rail:saveCopyAs(root..'/CanchaGuia.png');rail:close()
local post=Sprite(16,128,ColorMode.RGB);local timber=Image(16,128,ColorMode.RGB)
for y=0,127 do for x=0,15 do local n=math.random(-4,4);timber:drawPixel(x,y,(x==0 or x==15) and rgb(30,24,19) or rgb(80+n+(x<5 and 10 or 0),55+n,31+n)) end end
post.cels[1].image=timber;post:saveAs(root..'/PilarMadeira.aseprite');post:saveCopyAs(root..'/PilarMadeira.png');post:close()
print('Terra fina, saibro sem grade e guias contínuas de madeira exportados.')
