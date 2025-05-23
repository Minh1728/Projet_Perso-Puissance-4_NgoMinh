/*
Auteur : Minh Quang Ngo
Date de création : 10.03.2025
Date de modification : 05.05.2025
Description : Coder le puissance 4
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Projet_Perso_Puissance_4_NgoMinh
{
    public partial class Form1 : Form
    {
        //La variable que j'utilserai beaucoup dans ce projet.
        bool is_red;
        // Tableau représentant l'état des cellules (0 = vide, 1 = pièce rouge, 2 = pièce jaune)
        private int[,] game = new int[6, 7]; 
        // Liste pour stocker tous les boutons
        private Button[] buttons = new Button[7];


        public Form1()
        {
            //Le message du début lorsqu'on démarre le jeu
            InitializeComponent();
            game = new int[6, 7];

            // Un message est affiché pour le joueur 1 qui commence avec son choix pour les pièces et jouer
            DialogResult dialogResult = MessageBox.Show
            ("Joueur 1", "Choisissez les pièces entre Rouge ou Jaune", MessageBoxButtons.YesNo);

            // Si j'appuie oui, je choisis les pièces rouges avec un fond Navy un peu bleu foncé.
            if (dialogResult == DialogResult.Yes)
            {
                is_red = true; // Si le joueur 1 appuie sur OUi
                BackColor = Color.Navy; // Fond bleu foncé = Navy
            }

            // Sinon je choisis les pièces jaunes avec un fond vert.
            else
            {
                is_red = false; // Si le joueur 1 appuie Non
                BackColor = Color.Green; // Fond Vert
            }

            // la méthode pour commencer la partie
            StartGame();

        }


        //La partie ou les 2 joueurs jouent leurs pièces respectifs 
        private void btn_play(object sender, EventArgs e)
        {

            //Mettre à jour les boutons avec les tags et les messages.
            Button btn_clicked = (Button)sender;

            string tag = btn_clicked.Tag.ToString(); // mettre le string tag en btn_clicked.Tag.ToString
            MessageBox.Show("Tag du bouton cliqué : " + tag);  // Debug pour voir le tag
                                                               // Récupérer la colonne à partir du tag du bouton.

            char char_x = Convert.ToString(btn_clicked.Tag)[0]; 
            char char_y = Convert.ToString(btn_clicked.Tag)[2];

            int x = (int)char.GetNumericValue(char_x);
            int y = (int)char.GetNumericValue(char_y);
            
            //Les pièces rouges qui sont jouées lorsque je clique sur un bouton
            if (is_red == true)
            {                
                MessageBox.Show("Piece Rouge");// afficher un message pour le tour du joueur 1
                // Vérifier si il y a déjà une pièce posée
                int max_height = 0;

                //Une for pour faire la limite de la grille et du jeu avec les pièces rouges
                for (int i = 5; i > 0; i--) 

                {
                    if(game[i, y] == 0)
                    {
                        max_height = i;
                        break;
                    }
                }

                // Poser la pièce rouge
                DropPieceDown(max_height, y, "red");
                // Le tour du joueur 1
                is_red = false;
                MessageBox.Show("À vous de jouer, Joueur 2");
            }

            //Les pièces jaunes qui sont jouées lorsque je clique sur un bouton
            else
            {
                MessageBox.Show("Piece Jaune"); // afficher un message pour le tour du joueur 2
                // Vérifier si il y a déjà une pièce posée
                int max_height = 0;

                //Une for pour faire la limite de la grille et du jeu avec les pièces jaunes
                for (int i = 5; i > 0; i--)
                {
                    if (game[i, y] == 0)
                    {
                        max_height = i;
                        break;
                    }
                }

                // Poser la pièce jaune
                DropPieceDown(max_height, y, "yellow");
                // Le tour du joueur 2
                is_red = true;
                MessageBox.Show("À vous de jouer, Joueur 1");
            }

            // Vérifier la victoire si 4 pièces sont alignées
            //la méthode Checkwin va avec la méthode finish pour finir le jeu.
            if (CheckWin(x, y))
            {
                // Déclenche la fin de partie
                Finish(); 
            }
            // Si toutes les cases sont remplies, le jeu s'arrête
            else if (CheckDraw())  
            {
                // Si c'est un match nul, afficher un message et donne un choix de recommencer ou quitter
                var result = MessageBox.Show("Personne n'a gagné la partie, match nul. Voulez-vous recommencer ?",
                                              "Fin de la partie",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    // Si oui, je recommence la partie
                    ResetGame();  
                }
                else
                {
                    // Sinon, je ferme le jeu entièrement
                    Close();  
                }
            }

        }
        

        //La partie ou la pièce est posée et la plus importante du jeu
        private void DropPieceDown(int x, int y, string color)
        {
            // récupérer tous les boutons du jeu
            var controls = getControls(this);

            // récupérer le tag qui doit être dans le boutton
            string tagline = Convert.ToString(x) + "-" + Convert.ToString(y);

            // Parcourir tous les items de controls
            foreach (var item in controls)
            {
                // Si l'item a le même tag que notre tag 
                if(Convert.ToString(item.Tag) == tagline)
                {
                    // Mettre le bouton à la bonne couleur
                    if(color == "yellow")
                    {
                        // Couleur du bouton jaune
                        item.BackColor = Color.Yellow;
                        // Confirmer sa position ou il a joué
                        item.Enabled = false;
                        // Pièce jaune parce que le game x/y en 1 est pour les pièces jaunes
                        game[x, y] = 1; 
                    }
                    else if (color == "red")
                    {
                        // Couleur du bouton rouge
                        item.BackColor = Color.Red;
                        // Confirmer sa position ou il a joué
                        item.Enabled = false;
                        // Pièce rouge parce que le game x/y en 2 est pour les pièces rouges
                        game[x, y] = 2; 
                    }
                }
            }
        }
    

        // La condition d'égalité
        // Vérifier si toutes les cases sont remplies (match nul)
        private bool CheckDraw()
        {
            // Si toutes les pièces ont remplies toutes les colonnes
            for (int i = 0; i < 6; i++) 
            {
                //Parcours des colonnes
                for (int j = 0; j < 7; j++) 
                {
                    //Si une case est vide
                    if (game[i, j] == 0)  
                    {
                        //Il reste des cases vides
                        return false;  
                    }
                }
            }
            // Toutes les cases sont remplies
            return true; 
        }


        // Cette méthode vérifie si le joueur 1 ou 2 a gagné après avoir joué la dernière position (lastX, lastY)
        private bool CheckWin(int lastrow, int lastcol)
        {
            // Récupère le joueur actuel (1 = jaune, 2 = rouge)
            int player = game[lastrow, lastcol];

            // Si aucune pièce n'a été jouée à cet emplacement, retour immédiat
            if (player == 0) return false;

            // Tableau des directions à vérifier :
            // Chaque direction est un tableau [dx, dy]
            int[][] directions = new int[][]
            {
                // ↔ Direction Horizontal (Droite ou gauche)
                new int[] { 0, 1 },  
                // ↕ Direction Vertical (Haut ou bas)
                new int[] { 1, 0 },
                // ↘ Direction Diagonale descendante (haut-gauche vers bas-droit)
                new int[] { 1, 1 }, 
                // ↗ Direction Diagonale montante (bas-gauche vers haut-droit)
                new int[] { 1, -1 }   
            };

            // Vérifie chaque direction pour compter les pièces alignées
            foreach (var dir in directions)
            {
                // On commence à 1 pour compter la pièce que le joueur vient de poser
                int count = 1; 

                // On compte les pièces alignées dans une direction non-opposée
                // (exemple : à droite, en bas, en diagonale...)
                count += CountInDirection(lastrow, lastcol, dir[0], dir[1], player);

                // On compte aussi les pièces dans la direction opposée (ex : à gauche, en haut...)
                count += CountInDirection(lastrow, lastcol, -dir[0], -dir[1], player);

                // Si on atteint 4 pièces alignées ou plus, le joueur gagne
                if (count >= 4)
                    // Confirme la victoire
                    return true; 
            }

            // Si aucune des directions n'a donné 4 pièces alignées, pas de victoire
            return false; 
        }


        // Cette méthode compte le nombre de pièces alignées dans une direction indiquée
        private int CountInDirection(int startX, int startY, int dx, int dy, int player)
        {
            //Je déclare la variable count pour le nombre des pièces
            int count = 0;

            // Commence une case plus loin dans la direction indiquée
            int x = startX + dx;
            int y = startY + dy;

            // Continue tant qu'on est dans les limites du tableau et que les cases appartiennent au même joueur
            while (x >= 0 && x < 6 && y >= 0 && y < 7 && game[x, y] == player)
            {
                // Incrémente le nombre de pièces alignées
                count++;
                // On avance d'une case dans la direction choisie (gauche, droite, haut, bas ou diagonale)
                x += dx;
                y += dy; 
            }
            // Retourne le nombre de pièces trouvées dans cette direction
            return count; 
        }


        // Fonction pour gérer la fin de la partie et offrir au gagnant de recommencer ou quitter
        private void Finish()
        {
            // Afficher un message pour gagner.
            var result = MessageBox.Show("Félicitations vainqueur, tu as gagné ! Veux-tu quitter ?",
                                          "Fin de la partie",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Si oui, le jeu se ferme.
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Sinon, le jeu recommence.
                ResetGame();
            }
        }


        // Méthode pour commencer la partie du puissance 4
        private void StartGame()
        {
            // Afficher un message pour commencer
            MessageBox.Show("La partie commence ! Bonne partie");
            // Confirme le bouton commencer
            btn_Start.Enabled = false; 
        }


        // Recommence la partie du puissance 4
        private void ResetGame()
        {
            // Fais recommencer la partie
            System.Windows.Forms.Application.Restart();
            //Reprends tous les controls (bouttons, labels, panels, etc...)
            // d'un formulaire passé en paramètre 
            var controls = getControls(this);
        }


        //Quitter la partie si le joueur en a marre ou si le code ne marche pas dans le Puissance 4
        private void btn_quit(object sender, EventArgs e)
        {
            // Quitte immédiatement la partie
            Close(); 
        }


        // Récupérer tous les controls (bouttons, labels, panels, etc...)
        // d'un formulaire passé en paramètre et bout de code récupérer
        // lors du deuxième stage d'informatique pour la bataille navale
        private List<Control> getControls(Control controlPanel)
        {
            List<Control> controls = new List<Control>();

            foreach (Control item in controlPanel.Controls)
            {
                controls.Add(item);
            }

            return controls;
        }
    }
}

