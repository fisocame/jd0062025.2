# jd0062025.2

# Projeto 3D - Realidade Aumentada e Realidade Virtual para Jogos Digitais

Este repositório contém o projeto desenvolvido durante as aulas da disciplina **RA e RV para Jogos Digitais**.  
Aqui você encontrará o código-fonte, assets e atualizações organizadas **aula por aula**.

---

## Requisitos

- **Unity**: Versão 6000.0.48f1 (confira no Unity Hub a versão correta antes de abrir)  
- **Git**: instalado [Download aqui](https://git-scm.com/downloads)  
- **Git LFS**: necessário para baixar arquivos grandes (texturas, modelos 3D, sons, vídeos).  
  > Rode uma vez no seu PC:  
  > ```bash
  > git lfs install
  > ```

---

## Como baixar o projeto

### Opção 1 – Download direto
1. Vá até a aba **Releases** aqui no GitHub.  
2. Escolha a versão da aula (ex.: `Aula 02`, `Aula 03` etc.).  
3. Baixe o arquivo `.zip` e extraia no seu computador.  
4. Abra no **Unity Hub** → **Add Project from Disk** → selecione a pasta.

### Opção 2 – Usando Git

No terminal:
```bash
git clone https://github.com/fisocame/jd0062025.2.git
cd jd0062025.2
```

Para mudar para uma aula específica:

```bash
git fetch --tags
git checkout tags/v0.1-aula-01
```

## **Acompanhando Aula por Aula**

Cada aula tem um checkpoint (tag) marcado como:

> v0.1-aula-01 → Onde a gente parou na Aula 01

> v0.2-aula-02 → Onde a gente parou na Aula 02

> v0.3-aula-03 → Onde a gente parou na Aula 03

Essas tags também estão publicadas como Releases → você pode baixar o .zip direto sem precisar usar Git.

## **Fluxo de trabalho (para quem usa Git)**

Atualizar para a última versão
```bash
git pull
```
Baixar versão de uma aula específica
```bash
git fetch --tags
git checkout tags/v0.2-aula-02
```
Voltar para a versão atual (main)
```bash
git checkout main
```

## **Notas importantes**

- Não edite nem envie nada para este repositório sem autorização.
- Se quiser apenas estudar, use Download ZIP ou git clone em modo leitura.
- As pastas Library, Temp, Build, Obj e Logs são geradas pelo Unity automaticamente e não estão incluídas aqui.

## **Organização do professor**

- Branch main: contém a versão estável consolidada.
- Tags/Releases: checkpoints por aula.
- Branches auxiliares podem ser criadas para conteúdos extras.

