local root=app.params.root
local source=app.open(root..'/FrequentadoresSinuca.png')
local atlas=source.cels[1].image
local names={'Frequentador','Chico','Ramiro','Motorista'}
for row=0,3 do
  local s=Sprite(128,128,ColorMode.RGB)
  for pose=0,7 do
    local im=Image(128,128,ColorMode.RGB)
    im:drawImage(atlas,Point(-pose*128,-row*128))
    if pose==0 then s.cels[1].image=im else s:newEmptyFrame();s:newCel(s.layers[1],pose+1,im,Point(0,0)) end
    s.frames[pose+1].duration=.24
  end
  s:newTag(1,1).name='frente_repouso'
  s:newTag(2,3).name='frente_conversa'
  s:newTag(4,4).name='frente_bebendo'
  s:newTag(5,5).name='perfil_repouso'
  s:newTag(6,6).name='perfil_conversa'
  s:newTag(7,7).name='perfil_bebendo'
  s:newTag(8,8).name='costas_repouso'
  s:saveAs(root..'/'..names[row+1]..'.aseprite')
  s:close()
end
source:close()
