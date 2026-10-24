-- Arte original: soleiras vistas de cima, capachos e escada rasa da Sociedade.
-- Sem letras/setas de interface; 32 px/u, fontes editáveis, exportação determinista.
local root=assert(app.params.root);local pc=app.pixelColor
local function c(r,g,b,a)return pc.rgba(r,g,b,a or 255)end
local function box(im,x,y,w,h,col)for yy=y,y+h-1 do for xx=x,x+w-1 do im:drawPixel(xx,yy,col)end end end
local function save(im,name)
 local s=Sprite(im.width,im.height,ColorMode.RGB);s.layers[1].name=name..' · matéria e desgaste';s.cels[1].image=im
 s:saveAs(root..'/Arte/Producao/Acessos/'..name..'.aseprite');s:saveCopyAs(root..'/Projeto Jogo 2D/Assets/Resources/Art/Acessos/'..name..'.png');s:close()
end
for variant,name in ipairs({'SaidaComercio','SaidaCasa','SaidaSociedade','SaidaPublica','SaidaServico'})do
 local im=Image(64,40,ColorMode.RGB);math.randomseed(2004+variant)
 -- Sombra discreta da soleira; capacho do lado de dentro (topo da imagem).
 box(im,2,31,60,6,c(12,13,12,80));box(im,2,31,60,3,c(39,34,28))
 box(im,3,28,58,4,c(111,101,78));box(im,4,28,56,1,c(167,153,115));box(im,4,30,56,1,c(76,69,55))
 local base=variant==1 and {57,66,59} or variant==2 and {124,79,48} or variant==3 and {110,88,52} or variant==4 and {66,76,70} or {68,65,57}
 box(im,6,4,52,23,c(18,20,17,160));box(im,7,3,50,22,c(base[1]-22,base[2]-19,base[3]-15))
 for y=5,22 do for x=9,54 do
  local n=math.random(-4,4);local weave=((x+y)%3==0 and 6 or 0)
  if variant==1 or variant==5 then weave=(x%4==0 and -15 or y%4==0 and -6 or 2) end
  if variant==2 and (y==7 or y==19 or x==12 or x==51)then weave=24 end
  if variant==3 and ((x+2*y)%5==0)then weave=12 end
  im:drawPixel(x,y,c(base[1]+n+weave,base[2]+n+weave,base[3]+n+weave))
 end end
 if variant==2 then
  for x=8,55,3 do box(im,x,1,1,2,c(164,125,76));box(im,x,25,1,2,c(147,105,64))end
 elseif variant==4 then
  box(im,12,7,40,1,c(116,123,102));box(im,12,20,40,1,c(116,123,102));box(im,12,8,1,12,c(116,123,102));box(im,51,8,1,12,c(116,123,102))
 end
 -- Pequenas lascas na pedra/metal, nada brilhante como um portal.
 box(im,11,29,4,1,c(139,129,102));box(im,45,30,3,1,c(135,121,94))
 save(im,name)
end
local im=Image(80,96,ColorMode.RGB);math.randomseed(85)
-- Indo do salão (base) à sala rebaixada (topo): três pisos largos de cimento.
for step=0,2 do
 local y=6+step*27;local shade=step*9
 for yy=y,y+25 do for x=10,69 do
  local n=math.random(-3,3);local worn=(x>28 and x<53)and 4 or 0
  im:drawPixel(x,yy,c(127+shade+n+worn,119+shade+n+worn,96+shade+n+worn))
 end end
 -- Espelho vertical de oito pixels, com oclusão de contato na base.
 box(im,10,y+18,60,8,c(66+shade,61+shade,49+shade));box(im,11,y+18,58,2,c(101+shade,93+shade,72+shade))
 box(im,10,y+24,60,2,c(32+shade,30+shade,25+shade));box(im,11,y+17,58,1,c(181,169,136))
 box(im,28,y+16,27,1,c(153+shade,144+shade,117+shade))
end
-- Faces de contenção e corrimãos baixos de madeira, em perspectiva top-down.
for _,x in ipairs({3,70})do
 box(im,x,3,7,87,c(43,36,27));box(im,x+1,3,5,84,c(95,73,45));box(im,x+1,3,2,84,c(145,113,69))
 for y=10,84,24 do box(im,x-1,y,9,3,c(48,41,30));box(im,x,y,7,1,c(166,128,77))end
end
box(im,10,87,60,4,c(42,37,28,110));save(im,'EscadaCancha')
local wall=Image(64,64,ColorMode.RGB);math.randomseed(82)
for y=0,63 do for x=0,63 do
 local n=math.random(-2,2)
 if y<38 then wall:drawPixel(x,y,c(135+n,132+n,112+n))
 else local seam=(x%9==0 or x%9==8)and -17 or 0;local grain=math.floor(math.sin(x*2+y*.3)*3)
 wall:drawPixel(x,y,c(83+n+seam+grain,59+n+seam+grain,39+n+seam+grain))end
end end
box(wall,0,0,64,3,c(62,49,34));box(wall,0,3,64,1,c(161,130,85));box(wall,0,36,64,3,c(62,48,32));box(wall,0,36,64,1,c(154,120,75))
box(wall,12,20,1,9,c(113,110,95));box(wall,13,28,3,1,c(113,110,95));save(wall,'ParedeCancha')
local cap=Image(64,16,ColorMode.RGB)
for y=0,15 do for x=0,63 do local n=math.random(-3,3)
 local r,g,b=126+n,112+n,85+n
 if y==0 then r,g,b=173,153,114 elseif y>10 then r,g,b=67+n,58+n,43+n elseif x%32==0 then r,g,b=92,82,61 end
 cap:drawPixel(x,y,c(r,g,b))end end
save(cap,'CapeamentoParede')
-- Encosto visto por trás, voltado para o saibro ao norte. Não espelhar pés no eixo Y.
local bench=Image(128,96,ColorMode.RGB)
box(bench,10,48,109,9,c(35,29,23));box(bench,11,47,107,6,c(117,80,47));box(bench,12,47,105,1,c(162,117,67))
for _,x in ipairs({17,104})do box(bench,x,55,7,29,c(36,30,24));box(bench,x+1,56,3,26,c(83,57,35))end
for y=58,74,8 do
 box(bench,8,y,112,7,c(33,28,23));box(bench,9,y,110,5,c(100,68,41));box(bench,10,y,108,1,c(145,102,58))
 for x=20,106,86 do box(bench,x,y+2,1,1,c(186,165,118))end
end
save(bench,'BancoCanchaSul')
local shade=Image(64,24,ColorMode.RGB)
for y=0,23 do for x=0,63 do local r=((x-31.5)/31.5)^2+((y-11.5)/11.5)^2
 if r<1 then shade:drawPixel(x,y,c(15,17,16,math.floor((1-r)*74/8)*8))end end end
save(shade,'SombraContato')
print('Acessos: cinco soleiras com capacho e escada rasa de três degraus.')
