﻿/*
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
        bool is_red; //La variable que j'utilserai beaucoup dans ce projet.
        private int[,] game = new int[6, 7]; // Tableau représentant l'état des cellules (0 = vide, 1 = pièce rouge, 2 = pièce jaune)
        // Liste pour stocker tous les boutons
        private Button[] buttons = new Button[7];

        public Form1()
        {
            //Le message du début lorsqu'on démarre le jeu
            InitializeComponent();
            game = new int[6, 7];

            DialogResult dialogResult = MessageBox.Show("Joueur 1", "Choisissez les pièces entre Rouge ou Jaune", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)// si j'appuie oui, je choisis les pièces rouges avec un fond Navy un peu bleu foncé.
            {
                is_red = true;
                BackColor = Color.Navy; // Fond bleu foncé = Navy
            }

            else // sinon je choisis les pièces jaunes avec un fond vert.
            {
                is_red = false;
                BackColor = Color.Green; // Fond Vert
            }

            StartGame();// la méthode pour commencer la partie

        }

        private void btn_play(object sender, EventArgs e)
        {

            //Mettre à jour les boutons avec les tags et les messages.
            Button btn_clicked = (Button)sender;

            string tag = btn_clicked.Tag.ToString();
            MessageBox.Show("Tag du bouton cliqué : " + tag);  // Debug pour voir le tag
                                                               // Récupérer la colonne à partir du tag du bouton.

            char char_x = Convert.ToString(btn_clicked.Tag)[0];
            char char_y = Convert.ToString(btn_clicked.Tag)[2];

            int x = (int)char.GetNumericValue(char_x);
            int y = (int)char.GetNumericValue(char_y);

            //La pièce jouée lorsqu'on clique sur un bouton
            // Mettre à jour le tableau "game" avec la pièce du joueur
            if (is_red == true)
            {
                MessageBox.Show("Piece Rouge");// afficher un message pour le tour du joueur 1
                // Vérifier si il y a déjà une pièce posée
                int max_height = 0;

                for (int i = 5; i > 0; i--)

                {
                    if(game[i, y] == 0)
                    {
                        max_height = i;
                        break;
                    }
                }

                // Poser la pièce
                DropPieceDown(max_height, y, "red");

                is_red = false;
            }

            else
            {
                MessageBox.Show("Piece Jaune"); // afficher un message pour le tour du joueur 2
                // Vérifier si il y a déjà une pièce posée
                int max_height = 0;

                for (int i = 5
                    ; i > 0; i--)
                {
                    if (game[i, y] == 0)
                    {
                        max_height = i;
                        break;
                    }
                }

                // Poser la pièce
                DropPieceDown(max_height, y, "yellow");

                is_red = true;
            }
            


            // Vérifier la victoire après chaque coup
            if (CheckWin(x, y))//la méthode Checkwin va avec la méthode finish pour finir le jeu.
            {
                Finish();  // Déclenche la méthode de fin de partie
            }
            else if (CheckDraw())  // Si toutes les cases sont remplies, le jeu s'arrête
            {
                // Si c'est un match nul, afficher un message et donne un choix de recommencer ou quitter
                var result = MessageBox.Show("Personne n'a gagné la partie, match nul. Voulez-vous recommencer ?",
                                              "Fin de la partie",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    ResetGame();  // Recommence la partie
                }
                else
                {
                    Close();  // Fermer le jeu entièrement
                }
            }


        }
        
        private void DropPieceDown(int x, int y, string color)
        {
            // récupérer tous les boutons du jeu
            var controls = getControls(this);

            // récupérer le tag qui doit être dans le boutton
            string tagline = Convert.ToString(x) + "-" + Convert.ToString(y);

            // Parcourir tous les items de controls
            foreach (var item in controls)
            {
                // Si l'item à le même tag que notre tag
                if(Convert.ToString(item.Tag) == tagline)
                {
                    // Mettre le bouton à la bonne couleur
                    if(color == "yellow")
                    {
                        item.BackColor = Color.Yellow;
                        item.Enabled = false;
                        game[x, y] = 1;
                    }
                    else if (color == "red")
                    {
                        item.BackColor = Color.Red;
                        item.Enabled = false;
                        game[x, y] = 2;
                    }
                }
            }
        }
    
        // La condition d'égalité
        // Vérifier si toutes les cases sont remplies (match nul)
        private bool CheckDraw()
        {
            for (int i = 0; i < 8; i++)  // Parcours des lignes
            {
                for (int j = 0; j < 7; j++)  //Parcours des colonnes
                {
                    if (game[i, j] == 0)  //Si une case est vide
                    {
                        return false;  //Il reste des cases vides
                    }
                }
            }
            return true;  // Toutes les cases sont remplies
        }


        // Cette méthode vérifie si le joueur a gagné après avoir joué à la position (lastX, lastY)
        private bool CheckWin(int lastX, int lastY)
        {
            // Récupère le joueur actuel (1 = jaune, 2 = rouge)
            int player = game[lastX, lastY];

            // Si aucune pièce n'a été jouée à cet emplacement, retour immédiat
            if (player == 0) return false;

            // Tableau des directions à vérifier :
            // Chaque direction est un tableau [dx, dy]
            int[][] directions = new int[][]
            {
        new int[] { 0, 1 },   // ↔ Horizontal : gauche-droite
        new int[] { 1, 0 },   // ↕ Vertical : haut-bas
        new int[] { 1, 1 },   // ↘ Diagonale descendante (haut-gauche vers bas-droit)
        new int[] { 1, -1 }   // ↗ Diagonale montante (bas-gauche vers haut-droit)
            };

            // Vérifie chaque direction pour compter les pièces alignées
            foreach (var dir in directions)
            {
                int count = 1; // Commence à 1 pour inclure la pièce actuelle

                // Compte dans la direction normale (ex: vers la droite, vers le bas...)
                count += CountInDirection(lastX, lastY, dir[0], dir[1], player);

                // Compte dans la direction opposée (ex: vers la gauche, vers le haut...)
                count += CountInDirection(lastX, lastY, -dir[0], -dir[1], player);

                // Si on atteint 4 pièces alignées ou plus, le joueur gagne
                if (count >= 4)
                    return true;
            }

            // Si aucune des directions n'a donné 4 pièces alignées, pas de victoire
            return false;
        }


        // Cette méthode compte le nombre de pièces alignées dans une direction donnée
        private int CountInDirection(int startX, int startY, int dx, int dy, int player)
        {
            int count = 0;

            // Commence une case plus loin dans la direction donnée
            int x = startX + dx;
            int y = startY + dy;

            // Continue tant qu'on est dans les limites du tableau et que les cases appartiennent au même joueur
            while (x >= 0 && x < 6 && y >= 0 && y < 7 && game[x, y] == player)
            {
                count++; // Incrémente le nombre de pièces alignées
                x += dx; // Avance dans la direction horizontale ou verticale ou diagonale
                y += dy;
            }

            return count; // Retourne le nombre de pièces trouvées dans cette direction
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
            btn_Start.Enabled = false;
        }

        // Recommence la partie du puissance 4
        private void ResetGame()
        {
            System.Windows.Forms.Application.Restart();
            var controls = getControls(this);
        }

        //Quitter la partie si le joueur ragequit ou si ça ne marche pas
        private void btn_quit(object sender, EventArgs e)
        {
            Close();
        }


        // Récupérer tous les controls (bouttons, labels, panels, etc...) d'un formulaire passé en paramètre
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

