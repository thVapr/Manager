import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PartService } from '../../services/part/part.service';
import { Router } from '@angular/router';
import { Part } from 'src/app/components/models/part'
import { AuthService } from '../../services/auth/auth.service';
import { TreeDragDropService, TreeNode } from 'primeng/api';
import { TreeNodeDropEvent, TreeNodeSelectEvent, TreeNodeUnSelectEvent } from 'primeng/tree';
import { ButtonModule } from 'primeng/button';
import { StepperModule } from 'primeng/stepper';
import { PartType } from '../models/part-type';
import { AppComponent } from 'src/app/app.component';

@Component({
    selector: 'app-project',
    templateUrl: './part.component.html',
    styleUrls: ['./part.component.scss'],
    standalone: false,
    providers: [TreeDragDropService, ButtonModule, StepperModule]
})
export class PartComponent {
  addPartForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(20)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    type: new FormControl('1', [Validators.required])
  });
  
  items: TreeNode[] = [];
  isPartLeader : boolean = false;
  parts : Part[] = [];
  selectedNode : TreeNode | null = null;
  partTypes : PartType[] = [];
  isPartDialogActive : boolean = false;

  constructor(public authService: AuthService,
              public partService: PartService,
              public router : Router,
              public main : AppComponent) {}

  onChanges(event: TreeNodeDropEvent): void {
    this.partService
      .updateHierarchy(this.convertThreeNodesToParts(this.items))    
        .subscribe({
        next: () => {
        },
        error: (error) => console.error('failed', error)
    });;
  }
              
  ngOnInit(): void {
    const partId = this.partService.getPartId();
    const id = this.authService.getId();
    this.partService.getAllAccessible().subscribe({
      next: (parts) => {
        this.parts = parts;
        this.items = this.convertPartsToTreeNodes(parts);
        this.selectedNode = this.findNode(this.items);
        this.partService.getTypes().subscribe({
          next: (types) => 
          {
            this.partTypes = types;
          }
        })
      }
    });

    if (partId !== null && id !== null) {
      this.partService.getPartById(partId).subscribe({
        next: (part) => {
          this.partService.hasPrivileges(
            this.authService.getId(),
            part.id!,
            5).subscribe((response) => this.isPartLeader = response);
        },
        error: () => this.isPartLeader = false
      });
    }
  }

  private convertPartsToTreeNodes(parts: Part[]): TreeNode[]
  {
    return parts.map(part => {
        const treeNode: TreeNode = {
            key: part.id,
            label: part.name,
            data: part,
            children: this.convertPartsToTreeNodes(part.parts || []),
            expanded: true
        };
        switch (part.typeId)
        {
          case 1:
            treeNode.icon = "bi bi-box";
            break;
          case 2:
            treeNode.icon = "bi bi-people-fill";
            break;
          case 3:
            treeNode.icon = "bi bi-kanban";
            break;
          default:
            treeNode.icon = "bi bi-app"
            break;
        }
        return treeNode;
    });
  }

  private convertThreeNodesToParts(nodes: TreeNode[], level : number = 0, mainPartId : string = "00000000-0000-0000-0000-000000000000"): Part[]
  {
    if (level === 0)
    {
      let minLevel = this.parts.reduce((min, part) => {
        const currentLevel = part.level || 0;
        return currentLevel < min ? currentLevel : min ;
      }, 0);
      level = minLevel;
    }

    return nodes.map(node => {
      const part: Part = node.data;
      part.mainPartId = mainPartId
      part.level = level;
      part.parts = this.convertThreeNodesToParts(node.children!, level + 1, part.id);
      
      return part;
    });
  }
  
  onSubmit() : void {
    const name = this.addPartForm.value.name;
    const description = this.addPartForm.value.description;
    const type : number = Number(this.addPartForm.value.type);
    
    this.partService.addPart(name!, description ?? "", type!)
    .subscribe({
      next: () => {
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
          this.router.navigate(['parts']);
        });
      },
      error: (error) => console.error('failed', error)
     });
  }

  private findNode(tree: TreeNode[]) : TreeNode | null
  {
    for (let node of tree) {
      if (node.key === this.partService.getPartId()) {
        return node;
      }

      if (node.children) {
        const found = this.findNode(node.children);
        if (found) return found;
      }
    }

    return null;
  }

  choosePart(event : TreeNodeSelectEvent) : void {
    if (event.node.data != null) {
      this.partService.setPartId(event.node.data.id);
      this.partService.setPartName(event.node.data.name);
      this.main.updateMenuItems();
    }

    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate(['parts']);
    });
  }

  unselectPart(event : TreeNodeUnSelectEvent) : void {
    this.partService.removePartData();
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate(['parts']);
      this.main.updateMenuItems();
    });
  }

  showAddPartDialog() {
    this.isPartDialogActive = true;
  }
  
  cancelDialog() {
    this.isPartDialogActive = false;
    this.addPartForm.reset();
    this.addPartForm.patchValue({
      type : '1'
    });
  }
}