-- Peças autorais pequenas para a planta R2. Pivô comum: pés a 12 px da base.
local root=assert(app.params.root);local pc=app.pixelColor
local function c(r,g,b) return pc.rgba(r,g,b,255) end
local ink=c(30,27,24);local wood=c(98,65,40);local light=c(145,105,64);local dark=c(61,43,31)
local metal=c(103,112,111);local shine=c(175,180,160)
local function rect(im,x,y,w,h,col) for j=y,y+h-1 do for i=x,x+w-1 do im:drawPixel(i,j,col) end end end
local function oval(im,cx,cy,rx,ry,col) for y=cy-ry,cy+ry do for x=cx-rx,cx+rx do if ((x-cx)/rx)^2+((y-cy)/ry)^2<=1 then im:drawPixel(x,y,col) end end end end
local function table(im)
 rect(im,13,44,102,8,ink);rect(im,15,42,98,8,wood)
 for y=42,48,3 do rect(im,15,y,98,1,light) end
 for x=18,106,88 do rect(im,x,50,7,30,ink);rect(im,x+1,51,4,27,wood) end
 rect(im,24,66,82,4,dark)
end
for _,name in ipairs({'ApoioAguaQuente','BancadaBochas','SuporteBochas','BancoCancha','GrelhaDrenagem'}) do
 local im=Image(128,96,ColorMode.RGB)
 if name=='ApoioAguaQuente' then
  table(im);oval(im,31,33,12,10,ink);oval(im,31,32,10,8,metal);oval(im,29,30,7,5,shine)
  rect(im,25,22,13,3,metal);rect(im,29,19,5,3,ink);rect(im,41,28,6,3,metal);rect(im,45,24,3,5,metal)
  rect(im,17,23,3,12,ink);rect(im,20,21,4,3,ink)
  oval(im,61,18,7,4,ink);rect(im,54,18,15,22,ink);oval(im,61,39,7,3,ink)
  rect(im,56,18,11,22,c(54,77,61));rect(im,57,20,2,17,c(81,105,81));oval(im,61,18,5,2,metal);rect(im,58,13,7,4,metal)
  oval(im,79,38,7,5,dark);oval(im,79,34,6,3,c(126,88,49));rect(im,82,23,1,13,shine)
  rect(im,94,34,9,9,shine);rect(im,95,35,7,2,c(214,207,173))
 elseif name=='BancadaBochas' then
  table(im);rect(im,24,34,28,8,c(119,137,123));rect(im,26,34,4,9,c(152,164,138))
  oval(im,70,35,9,8,ink);oval(im,70,34,7,6,c(135,78,43));oval(im,67,32,3,2,c(177,122,65))
  rect(im,89,29,17,6,dark);rect(im,91,29,12,2,light);rect(im,98,34,2,8,wood)
 elseif name=='SuporteBochas' then
  for x=18,104,86 do rect(im,x,18,6,64,ink);rect(im,x+1,18,3,62,wood) end
  for y=28,64,18 do
   rect(im,19,y,90,7,ink);rect(im,20,y,88,3,light)
   for x=32,94,20 do oval(im,x,y-4,8,7,ink);oval(im,x,y-5,6,5,c(112+(x%3)*12,69,39));oval(im,x-2,y-7,2,1,light) end
  end
 elseif name=='BancoCancha' then
  for y=31,47,8 do rect(im,8,y,112,7,ink);rect(im,9,y,110,4,wood);rect(im,10,y,108,1,light) end
  rect(im,9,55,111,9,ink);rect(im,10,54,109,5,wood);rect(im,11,54,107,1,light)
  for x=18,104,86 do rect(im,x,62,6,20,dark);rect(im,x+1,62,2,18,wood) end
 elseif name=='GrelhaDrenagem' then
  rect(im,21,38,87,24,ink);rect(im,23,40,83,20,metal)
  for x=27,102,7 do rect(im,x,42,3,16,dark) end
  for _,p in ipairs({{24,40},{103,40},{24,57},{103,57}}) do im:drawPixel(p[1],p[2],shine) end
 end
 local s=Sprite(128,96,ColorMode.RGB);s.cels[1].image=im;s:saveAs(root..'/'..name..'.aseprite');s:saveCopyAs(root..'/'..name..'.png');s:close()
end
print('R2: copa, suporte, bancada, banco e grelha em peças separadas.')
local im=Image(64,64,ColorMode.RGB)
for y=0,63 do for x=0,63 do
 local hash=(x*17+y*43+x*y*3)%127;local value=hash<8 and -5 or hash>120 and 4 or 0
 local col=y<41 and c(134+value,128+value,108+value) or c(74+value,53+value,35+value)
 if y==40 or y==43 then col=c(159,119,73) end
 if y>44 and x%8==0 then col=c(49,37,27) end
 if y>44 and x%8==1 then col=c(105,76,46) end
 im:drawPixel(x,y,col)
end end
local wall=Sprite(64,64,ColorMode.RGB);wall.cels[1].image=im;wall:saveAs(root..'/RebocoLambril.aseprite');wall:saveCopyAs(root..'/RebocoLambril.png');wall:close()
-- Caixas de papelão de depósito: faces, fita, dobras e impressão, separadas do estoque antigo de C1.
local im=Image(128,112,ColorMode.RGB)
local function box(x,y,w,h)
 rect(im,x,y,w,h,ink);rect(im,x+1,y+1,w-2,h-2,c(137,105,64))
 rect(im,x+2,y+2,w-4,3,c(175,139,86));rect(im,x+w-5,y+3,3,h-5,c(99,74,45))
 rect(im,x+3,y+h-3,w-6,1,c(182,143,86));rect(im,x+w//2-2,y+1,5,h-3,c(161,129,78))
 rect(im,x+4,y+8,12,9,c(197,186,145));rect(im,x+5,y+10,9,1,c(84,79,61));rect(im,x+5,y+13,7,1,c(106,98,72))
 for k=0,5 do rect(im,x+w-15+k*2,y+h-11,1,6,c(65,57,40)) end
 for k=0,7 do local xx=x+2+(k*11)%(w-5);local yy=y+2+(k*7)%(h-5);im:drawPixel(xx,yy,c(117,88,54)) end
end
for y=87,95,4 do rect(im,13,y,104,3,dark);rect(im,15,y,100,1,light) end
box(49,17,36,31);box(19,42,45,42);box(67,52,39,33)
rect(im,32,35,27,7,c(147,115,70));rect(im,32,35,27,1,c(190,152,91))
local stock=Sprite(128,112,ColorMode.RGB);stock.cels[1].image=im;stock:saveAs(root..'/EstoqueDeposito.aseprite');stock:saveCopyAs(root..'/EstoqueDeposito.png');stock:close()
