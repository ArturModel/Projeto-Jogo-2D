-- Monta evidência animada a partir de capturas reais; não recria o jogo.
local folder=assert(app.params.folder);local output=assert(app.params.output)
local first=app.open(folder..'/walk-00.png');local width=math.floor(first.width/2);local height=math.floor(first.height/2);first:close()
local result=Sprite(width,height,ColorMode.RGB)
for i=0,15 do
  local frame=app.open(folder..'/walk-'..string.format('%02d',i)..'.png');local source=frame.cels[1].image
  local image=Image(width,height,ColorMode.RGB)
  for y=0,height-1 do for x=0,width-1 do image:drawPixel(x,y,source:getPixel(x*2,y*2)) end end
  if i==0 then result.cels[1].image=image else result:newEmptyFrame();result:newCel(result.layers[1],i+1,image,Point(0,0)) end
  result.frames[i+1].duration=.08;frame:close()
end
result:saveCopyAs(output);result:close();print('GIF de capturas reais exportado; reprodução sem faixa de áudio.')
